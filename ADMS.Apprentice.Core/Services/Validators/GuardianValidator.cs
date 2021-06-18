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
        private readonly IValidatorExceptionBuilderFactory exceptionBuilderFactory;
        private readonly IAddressValidator addressValidator;
        private readonly IPhoneValidator phoneValidator;

        public GuardianValidator(
            IValidatorExceptionBuilderFactory exceptionBuilderFactory,
            IAddressValidator addressValidator,
            IPhoneValidator phoneValidator
        )
        {
            this.exceptionBuilderFactory = exceptionBuilderFactory;
            this.addressValidator = addressValidator;
            this.phoneValidator = phoneValidator;
        }

        public async Task<IValidatorExceptionBuilder> ValidateAsync(Guardian guardian)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            PhoneValidation(exceptionBuilder, guardian);
            EmailValidation(exceptionBuilder, guardian);
            await AddressValidation(exceptionBuilder, guardian);
            return exceptionBuilder;
        }

        private void PhoneValidation(IValidatorExceptionBuilder exceptionBuilder, Guardian guardian)
        {
            guardian.HomePhoneNumber = phoneValidator.ValidatePhone(exceptionBuilder, guardian.HomePhoneNumber, ValidationExceptionType.InvalidGuardianNumber);
            guardian.Mobile = phoneValidator.ValidatePhone(exceptionBuilder, guardian.Mobile, ValidationExceptionType.InvalidGuardianNumber);
            guardian.WorkPhoneNumber = phoneValidator.ValidatePhone(exceptionBuilder, guardian.WorkPhoneNumber, ValidationExceptionType.InvalidGuardianNumber);
        }

        private void EmailValidation(IValidatorExceptionBuilder exceptionBuilder, Guardian guardian)
        {
            if (guardian.EmailAddress.SanitiseUpper() != null)
            {
                if (!EmailAddressHelper.EmailValidation(guardian.EmailAddress.SanitiseUpper()))
                    exceptionBuilder.Add(ValidationExceptionType.InvalidEmailAddress);
            }
        }

        private async Task AddressValidation(IValidatorExceptionBuilder exceptionBuilder, Guardian guardian)
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
                exceptionBuilder.Add(ValidationExceptionType.AddressRecordNotFoundForGuardian);               
            }
            else
            {
                exceptionBuilder.AddExceptions(await addressValidator.ValidateAsync(guardian));
            }            
        }
    }
}