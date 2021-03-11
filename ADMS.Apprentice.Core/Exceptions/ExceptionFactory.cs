using System.Collections.Generic;
using System.Linq;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Database;
using Adms.Shared.Extensions;

namespace ADMS.Apprentice.Core.Exceptions
{
    public class ExceptionFactory : IExceptionFactory
    {
        private readonly IContextRetriever contextRetriever;

        public ExceptionFactory(IContextRetriever contextRetriever)
        {
            this.contextRetriever = contextRetriever;
        }

        public ValidationException CreateValidationException(ValidationExceptionType exceptionType, params ValidationExceptionType[] additionalExceptionTypes)
        {
            var exceptionTypes = new List<ValidationExceptionType> {exceptionType};
            if (additionalExceptionTypes != null)
                exceptionTypes.AddRange(additionalExceptionTypes);
            var validationErrors = exceptionTypes
                .Distinct()
                .Select(t => t.GetAttribute<ExceptionDetailsAttribute>())
                .Select(attr => new ValidationError(attr.ValidationRuleId, attr.Message));
            return new ValidationException(contextRetriever.GetContext(), validationErrors);
        }


        public NotFoundException CreateNotFoundException(string resourceType, string resourceKey)
        {
            return new NotFoundException(contextRetriever.GetContext(), resourceType, resourceKey);
        }
    }
}