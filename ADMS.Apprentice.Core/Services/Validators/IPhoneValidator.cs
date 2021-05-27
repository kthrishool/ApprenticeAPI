using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IPhoneValidator
    {
        void ValidatePhonewithType(Phone Phone);
        string ValidatePhone(string phoneNumber, ValidationExceptionType exception);
    }
}