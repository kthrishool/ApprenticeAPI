using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IPhoneValidator
    {
        void ValidatePhonewithType(IValidatorExceptionBuilder exceptionBuilder, Phone Phone);
        string ValidatePhone(IValidatorExceptionBuilder exceptionBuilder, string phoneNumber, ValidationExceptionType exception);
    }
}