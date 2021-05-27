using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Helpers;
using Adms.Shared;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class GuardianValidator : IGuardianValidator
    {
        private readonly IRepository repository;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IAddressValidator addressValidator;

        private readonly IPhoneValidator phoneValidator;

        public GuardianValidator(
            IRepository repository,
            IExceptionFactory exceptionFactory,
            IAddressValidator addressValidator,
            IPhoneValidator phoneValidator
        )
        {
            this.repository = repository;
            this.exceptionFactory = exceptionFactory;
            this.addressValidator = addressValidator;
            this.phoneValidator = phoneValidator;
        }

        public async Task ValidateAsync(Guardian guardian)
        {
            PhoneValidation(guardian);
            EmailValidation(guardian);
            await AddressValidation(guardian);
        }

        private void PhoneValidation(Guardian guardian)
        {
            guardian.HomePhoneNumber = phoneValidator.ValidatePhone(guardian.HomePhoneNumber, ValidationExceptionType.InvalidGuardianNumber);
            guardian.Mobile = phoneValidator.ValidatePhone(guardian.Mobile, ValidationExceptionType.InvalidGuardianNumber);
            guardian.WorkPhoneNumber = phoneValidator.ValidatePhone(guardian.WorkPhoneNumber, ValidationExceptionType.InvalidGuardianNumber);
        }

        private void EmailValidation(Guardian guardian)
        {
            if (guardian.EmailAddress.SanitiseUpper() != null)
            {
                if (!EmailAddressHelper.EmailValidation(guardian.EmailAddress.SanitiseUpper()))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidEmailAddress);
            }
        }

        private async Task AddressValidation(Guardian guardian)
        {
            if (guardian.StreetAddress1.Sanitise() == null
                && guardian.StreetAddress2.Sanitise() == null
                && guardian.StreetAddress3.Sanitise() == null
                && guardian.Locality.Sanitise() == null
                && guardian.Postcode.Sanitise() == null
                && guardian.StateCode.Sanitise() == null
                && guardian.SingleLineAddress.Sanitise() == null)
                return;
            // address is manditory so need to throw exception when its null
            if (guardian.StreetAddress1.Sanitise() == null
                || guardian.Locality.Sanitise() == null)
            {
                if (guardian.SingleLineAddress.Sanitise() == null)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.AddressRecordNotFoundForGuardian);
            }
            await addressValidator.ValidateAsync(guardian);
        }
    }
}