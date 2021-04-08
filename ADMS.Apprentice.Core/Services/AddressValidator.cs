using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Adms.Shared;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;

namespace ADMS.Apprentice.Core.Services
{
    public class AddressValidator : IAddressValidator
    {
        private readonly IRepository repository;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IReferenceDataClient referenceDataClient;

        public AddressValidator(
            IRepository repository,
            IExceptionFactory exceptionFactory,
            IReferenceDataClient referenceDataClient
        )
        {
            this.repository = repository;
            this.exceptionFactory = exceptionFactory;
            this.referenceDataClient = referenceDataClient;
        }

        public async Task<List<Address>> ValidateAsync(List<Address> addresses)
        {
            var validatedAddress = new List<Address>();

            foreach (Address address in addresses)
            {
                if (!string.IsNullOrEmpty(address.SingleLineAddress))
                    validatedAddress.Add(await ValidateSingleLineAddressAsync(address));
                else
                {
                    //Do the default code validations
                    ValidateDefaultCodesAsync(address);
                    //At this point we know we have a valid Locality, State and postcode. So use iGas to get the partical geolocation details
                    validatedAddress.Add(await ValidatePartialAddressAsync(address));
                }
            }

            return validatedAddress;
        }

        private void ValidateDefaultCodesAsync(Address address)
        {
            if (address == null)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);

            // is the single line code is empty we need to check if other details are valid.
            // if the postcode is there then validate it first
            if (string.IsNullOrWhiteSpace(address.Postcode) ||
                string.IsNullOrWhiteSpace(address.Locality) ||
                string.IsNullOrWhiteSpace(address.StateCode))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);
            if (address.Postcode?.Length != 4 || address.Postcode?.All(char.IsDigit) == false)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidPostcode);
            if (address.StateCode?.Length > 10)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidStateCode);
            if (string.IsNullOrWhiteSpace(address.StreetAddress1))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.StreetAddressLine1CannotBeNull);
            if (address.StreetAddress1?.Length > 80)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.StreetAddressExceedsMaxLength);
            if (address.StreetAddress2?.Length > 80)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.StreetAddressExceedsMaxLength);
            if (address.StreetAddress3?.Length > 80)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.StreetAddressExceedsMaxLength);
            if (address.Locality?.Length > 40)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.SuburbExceedsMaxLength);
        }

        private async Task<Address> ValidateSingleLineAddressAsync(Address address)
        {
            //Verify the address using iGas
            //If it is a valid single line address, iGas will return a record with geo location details         

            DetailAddressModel detailAddress = await referenceDataClient.GetDetailAddressByFormattedAddress(address.SingleLineAddress);
            //no chance to have detail address to be null, but in case of reasons..
            if (detailAddress == null) throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);

            string Sanitise(string s) => s.IsNullOrEmpty() ? null : s;
            //populate geo location + postcode suburb details to profile address from detailsAddress component                                  
            address.SingleLineAddress = detailAddress.FormattedAddress;
            address.StreetAddress1 = Sanitise(detailAddress.StreetAddressLine1);
            address.StreetAddress2 = Sanitise(detailAddress.StreetAddressLine2);
            address.StreetAddress3 = Sanitise(detailAddress.StreetAddressLine2);
            address.Locality = detailAddress.Locality;
            address.StateCode = detailAddress.State;
            address.Postcode = detailAddress.Postcode;
            address.GeocodeType = detailAddress.GeocodeType;
            address.Latitude = detailAddress.Latitude;
            address.Longitude = detailAddress.Longitude;
            address.Confidence = (short) detailAddress.Confidence;
            return address;
        }

        private async Task<Address> ValidatePartialAddressAsync(Address address)
        {
            //Verify the partial address using iGas. Partial address = Locality + State + postcode
            //If it is a valid, populate geo location details

            string formattedLocality = $"{address.Locality} {address.StateCode} {address.Postcode}"?.ToUpper();

            if (string.IsNullOrEmpty(formattedLocality))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound); //no chance of hitting this, but in case..

            PartialAddressModel partialAddress = await referenceDataClient.GetAddressByFormattedLocality(formattedLocality);

            if (partialAddress == null)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);

            //if iGas couldnt resolve the partial address
            if (string.IsNullOrEmpty(partialAddress.Locality) || string.IsNullOrEmpty(partialAddress.State) || string.IsNullOrEmpty(partialAddress.Postcode))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);

            if (partialAddress.Locality != address.Locality.ToUpper())
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.PostCodeLocalityMismatch);

            if (partialAddress.State != address.StateCode.ToUpper())
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.PostCodeStateCodeMismatch);

            if (partialAddress.Postcode != address.Postcode)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.PostCodeMismatch);
            //update address with geo location details
            //partial address doenst provide confidence info, but according to below info assigning confidence level of 4
            //The confidence level from GNAF for the address and Geocode.Should be used in conjunction with Geo Code Type
            //- 1 = The address is not contained in any of three major contributors / sources of addresses(GNAF)
            //0 = The address is contained in only one of the three major contributors / sources of addresses(GNAF)
            //1 = The address is contained in two of the three major contributors / sources of addresses(GNAF)
            //2 = The address is contained in all three of the three major contributors / sources of addresses(GNAF)
            //3 = A Match was not found on Street Number.The Geocode returned will only be accurate to the Street(iGas)
            //4 = A Match was not found on Street.The Geocode returned will only be accurate to the Locality(iGas)
            //5 = Not found(iGas)

            address.Locality = partialAddress.Locality;
            address.StateCode = partialAddress.State;
            address.Postcode = partialAddress.Postcode;
            address.Latitude = partialAddress.Latitude;
            address.Longitude = partialAddress.Longitude;
            address.Confidence = 4;

            return address;
        }


        //private async Task<Address> CheckAddressFromReferenceAsync(Address message)
        //{
        //    PostcodeLocality[] autocompleteAddress = new PostcodeLocality[10];
        //    try
        //    {
        //        //AutocompleteAddressModel[] autocompleteAddress1 = await referenceDataClient.AutocompleteAddress(message.SingleLineAddress);
        //        autocompleteAddress = await referenceDataClient.GetRelatedCodesByPostCode(message.Postcode.Trim());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    // change the code to use reference data

        //    // first check if the postcode is valid
        //    if (autocompleteAddress.Length > 0)
        //    {
        //        if (autocompleteAddress.Count(c => c.ShortDescription.ToLower() == message.Locality.Trim().ToLower() || c.LongDescription.ToLower() == message.Locality.Trim().ToLower()) <= 0)
        //            throw exceptionFactory.CreateValidationException(ValidationExceptionType.PostCodeLocalityMismatch);
        //        else if (autocompleteAddress.Count(c => c.ShortDescription.ToLower() == message.Locality.Trim().ToLower() || c.LongDescription.ToLower() == message.Locality.Trim().ToLower()) == 1)
        //            message.StateCode = autocompleteAddress.SingleOrDefault(c => c.ShortDescription.ToLower() == message.Locality.Trim().ToLower() || c.LongDescription.ToLower() == message.Locality.Trim().ToLower())?.StateCode;
        //        else
        //            throw exceptionFactory.CreateValidationException(ValidationExceptionType.PostCodeStateCodeMismatch);
        //    }
        //    else
        //        throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidPostcode); //    Address will not be null as its only internally called.
        //    //return new Address();
        //}
    }
}