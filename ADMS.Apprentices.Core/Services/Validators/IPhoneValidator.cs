using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IPhoneValidator
    {
        void ValidatePhonewithType(ValidationExceptionBuilder exceptionBuilder, Phone Phone);
        string ValidatePhone(ValidationExceptionBuilder exceptionBuilder, string phoneNumber, ValidationExceptionType exception);
    }
}