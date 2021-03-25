using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;

namespace ADMS.Apprentice.Core.Services
{
    public class AddressValidator : IAddressValidator
    {
        private readonly ITyimsRepository tyimsRepository;
        private readonly IRepository repository;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IReferenceDataClient referenceDataClient;

        public AddressValidator(
            IRepository repository,
            ITyimsRepository tyimsRepository,
            IExceptionFactory exceptionFactory,
            IReferenceDataClient referenceDataClient
        )
        {
            this.tyimsRepository = tyimsRepository;
            this.repository = repository;
            this.exceptionFactory = exceptionFactory;
            this.referenceDataClient = referenceDataClient;
        }

        public async Task<Task> Validate(Profile message)
        {
            var validatedAddress = new List<Address>();

            foreach (Address messageAddress in message.Addresses)
            {
                validatedAddress.Add(await ValidateDefaultCodesAsync(messageAddress));
            }

            message.Addresses = validatedAddress;
            return Task.CompletedTask;
        }

        private async Task<Address> ValidateDefaultCodesAsync(Address message)
        {
            if (message == null)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);
            // if the singleLine is Present then we need to check it first.
            if (!string.IsNullOrWhiteSpace(message.SingleLineAddress))
                return await ValidateSingleLineAddressAsync(message);
            else
            {
                // is the single line code is empty we need to check if other details are valid.
                // if the postcode is there then validate it first
                if (string.IsNullOrWhiteSpace(message.Postcode) ||
                    string.IsNullOrWhiteSpace(message.Locality) ||
                    string.IsNullOrWhiteSpace(message.StateCode))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);
                if (message.Postcode?.Length != 4)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidPostcode);
                if (message.StateCode?.Length > 10)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidStateCode);
                if (message.StreetAddress1 == null)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.StreetAddressLine1CannotBeNull);
                if (message.StreetAddress1?.Length > 80)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.StreetAddressExceedsMaxLength);
                if (message.StreetAddress2?.Length > 80)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.StreetAddressExceedsMaxLength);
                if (message.StreetAddress3?.Length > 80)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.StreetAddressExceedsMaxLength);
                if (message.Locality?.Length > 40)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.SuburbExceedsMaxLength);
            }
            return message;
        }

        private async Task<Address> ValidateSingleLineAddressAsync(Address address)
        {
            //Verify the address using iGas
            //If it is a valid single line address, iGas will return just one record only
            

            AutocompleteAddressModel[] autocompleteAddress =  await referenceDataClient.AutocompleteAddress(address.SingleLineAddress);
            if (autocompleteAddress.Count() != 1) throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);

            //Get detailed address including geo location from the addressId. At this point we are sure that there is only one item in the autocompleteAddress array 
            DetailAddressModel detailAddress =  await referenceDataClient.GetDetailAddressById(autocompleteAddress[0].Id);
            //no chance to have detail address to be null, but in case of reasons..
            if (detailAddress == null) throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);

            //populate geo location + postcode suburb details to profile address from detailsAddress component                                  
            address.SingleLineAddress = detailAddress.FormattedAddress;
            address.StreetAddress1 = detailAddress.StreetAddressLine1;
            address.StreetAddress2 = detailAddress.StreetAddressLine2;
            address.StreetAddress3 = detailAddress.StreetAddressLine3;
            address.Locality = detailAddress.Locality;            
            address.StateCode = detailAddress.State;
            address.Postcode = detailAddress.Postcode;
            address.GeocodeType = detailAddress.GeocodeType;
            address.Latitude = detailAddress.Latitude;
            address.Longitude = detailAddress.Longitude;
            address.Confidence = detailAddress.Confidence;
            return address;            
        }

        private async Task<Address> CheckAddressWithTyimsAsync(Address message)
        {
            // change the code to use reference data
            var locationForPostCode = await tyimsRepository.GetPostCodeAsync(message.Postcode.Trim());
            // first check if the postcode is valid
            if (locationForPostCode.Any())
            {
                if (locationForPostCode.Any(c => c.LocalityCode == message.Locality.Trim()))
                {
                    if (locationForPostCode.Count(c => c.LocalityCode.ToLower() == message.Locality.Trim().ToLower()) == 1)
                        message.StateCode = locationForPostCode.SingleOrDefault(c => c.LocalityCode == message.Locality.Trim())?.StateCode;
                    else if (locationForPostCode.Count(c => c.LocalityCode == message.Locality.Trim() && c.StateCode.ToLower() == message.StateCode.Trim().ToLower()) != 1)
                        throw exceptionFactory.CreateValidationException(ValidationExceptionType.PostCodeStateCodeMissmatch);
                }
            }
            else
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.PostCodeStateCodeMissmatch); //    Address will not be null as its only internally called.
            return new Address();
        }
    }
}