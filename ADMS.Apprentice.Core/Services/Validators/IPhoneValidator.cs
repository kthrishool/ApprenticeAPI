using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IPhoneValidator
    {
        void ValidatePhonewithType(ValidationExceptionBuilder exceptionBuilder, Phone Phone);
        string ValidatePhone(ValidationExceptionBuilder exceptionBuilder, string phoneNumber, ValidationExceptionType exception);
    }
}