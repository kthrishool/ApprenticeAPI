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

        public async Task<Address> ManualAddressValidator(Address message)
        {
            await ValidateDefaultCodes(message);

            return new Address();
        }

        public async Task ValidateDefaultCodes(Address message)
        {
            if (message == null)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);
            // if the singleLine is Present then we need to check it first.
            if (!string.IsNullOrWhiteSpace(message.SingleLineAddress))
                ValidateSingleLineAddress(message);
            else
            {
                // is the single line code is empty we need to check if other details are valid.
                // if the postcode is there then validate it first
                if (string.IsNullOrWhiteSpace(message.Postcode) ||
                    string.IsNullOrWhiteSpace(message.Locality))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFound);

                await CheckAddressWithTyimsAsync(message);
            }
        }

        private void ValidateSingleLineAddress(Address message)
        {
            throw new NotImplementedException("Validation Not Implemented");
            // profileAddress will not be null as its only internally called .
        }

        private async Task CheckAddressWithTyimsAsync(Address message)
        {
            List<CodeLocalityPostcodesState> locationForPostCode = await tyimsRepository.GetPostCodeAsync(message.Postcode.Trim());
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
        }
    }
}