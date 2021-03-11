using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Exceptions
{
    public enum ValidationExceptionType
    {
        [ExceptionDetails("AP-VAL-0001", "Apprentice must be at least 12 years old")]
        InvalidApprenticeAge,
        [ExceptionDetails("AP-VAL-0002", "Invalid Apprentice profile type ")]
        InvalidApprenticeprofileType

    }
}