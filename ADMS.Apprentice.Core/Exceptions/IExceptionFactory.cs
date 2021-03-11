using ADMS.Services.Infrastructure.Core.Exceptions;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Exceptions
{
    [RegisterWithIocContainer]
    public interface IExceptionFactory
    {
        ValidationException CreateValidationException(ValidationExceptionType exceptionType, params ValidationExceptionType[] additionalExceptionTypes);

        NotFoundException CreateNotFoundException(string resourceType, string resourceKey);
    }
}
