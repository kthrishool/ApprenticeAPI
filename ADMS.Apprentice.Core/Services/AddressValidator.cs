using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services
{
    public class AddressValidator : IAddressValidator
    {
        private readonly ITyimsRepository tyimsRepository;
        private readonly IRepository repository;
        private readonly IExceptionFactory exceptionFactory;

        public AddressValidator(
            IRepository repository,
            ITyimsRepository tyimsRepository,
            IExceptionFactory exceptionFactory
        )
        {
            this.tyimsRepository = tyimsRepository;
            this.repository = repository;
            this.exceptionFactory = exceptionFactory;
        }

        public List<Address> Validate(Profile message)
        {
            var validatedAddress = new List<Address>();

            foreach (Address messageAddress in message.Addresses)
            {
                validatedAddress.Add(ValidateDefaultCodes(messageAddress));
            }


            return validatedAddress;
        }

        private Address ValidateDefaultCodes(Address message)
        {
            if (message == null)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);
            // if the singleLine is Present then we need to check it first.
            if (!string.IsNullOrWhiteSpace(message.SingleLineAddress))
                return ValidateSingleLineAddress(message);
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

        private Address ValidateSingleLineAddress(Address message)
        {
            throw new NotImplementedException("Validation Not Implemented");
            // profileAddress will not be null as its only internally called .
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