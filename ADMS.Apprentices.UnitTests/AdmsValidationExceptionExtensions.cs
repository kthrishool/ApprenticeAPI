using System.Linq;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;

namespace ADMS.Apprentices.UnitTests
{
    public static class AdmsValidationExceptionExtensions
    {
        public static bool IsForValidationRule(this AdmsValidationException validationException, ValidationExceptionType validationRule)
        {
            string validationRuleId = validationRule.GetAttribute<ExceptionDetailsAttribute>()?.ValidationRuleId;
            return validationException.ValidationDetails.Errors.Any(d => d.Key == validationRuleId);
        }
    }
}