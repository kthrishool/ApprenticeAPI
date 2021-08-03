using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Helpers;
using Adms.Shared;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class GuardianValidator : IGuardianValidator
    {
        private readonly IAddressValidator addressValidator;
        private readonly IPhoneValidator phoneValidator;

        public GuardianValidator(
            IAddressValidator addressValidator,
            IPhoneValidator phoneValidator
        )
        {
            this.addressValidator = addressValidator;
            this.phoneValidator = phoneValidator;
        }

        public async Task<ValidationExceptionBuilder> ValidateAsync(Guardian guardian)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();
            PhoneValidation(exceptionBuilder, guardian);
            EmailValidation(exceptionBuilder, guardian);
            await AddressValidation(exceptionBuilder, guardian);
            return exceptionBuilder;
        }

        private void PhoneValidation(ValidationExceptionBuilder exceptionBuilder, Guardian guardian)
        {
            guardian.HomePhoneNumber = phoneValidator.ValidatePhone(exceptionBuilder, guardian.HomePhoneNumber, ValidationExceptionType.InvalidGuardianNumber);
            guardian.Mobile = phoneValidator.ValidatePhone(exceptionBuilder, guardian.Mobile, ValidationExceptionType.InvalidGuardianNumber);
            guardian.WorkPhoneNumber = phoneValidator.ValidatePhone(exceptionBuilder, guardian.WorkPhoneNumber, ValidationExceptionType.InvalidGuardianNumber);
        }

        private void EmailValidation(ValidationExceptionBuilder exceptionBuilder, Guardian guardian)
        {
            if (guardian.EmailAddress.SanitiseUpper() != null)
            {
                if (!EmailAddressHelper.EmailValidation(guardian.EmailAddress.SanitiseUpper()))
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidEmailAddress);
            }
        }

        private async Task AddressValidation(ValidationExceptionBuilder exceptionBuilder, Guardian guardian)
        {
            if (guardian.StreetAddress1.Sanitise() == null
                && guardian.Locality.Sanitise() == null
                && guardian.Postcode.Sanitise() == null
                && guardian.StateCode.Sanitise() == null
                && guardian.SingleLineAddress.Sanitise() == null)
                return;
            // address is entered, Street, Locality, State and postcode is mandatory
            if (guardian.SingleLineAddress == null && (guardian.StreetAddress1 == null
                || guardian.Locality == null || guardian.StateCode == null || guardian.Postcode == null))
            {                
                exceptionBuilder.AddException(ValidationExceptionType.AddressRecordNotFoundForGuardian);               
            }
            else
            {
                exceptionBuilder.AddExceptions(await addressValidator.ValidateAsync(guardian));
            }            
        }
    }
}
