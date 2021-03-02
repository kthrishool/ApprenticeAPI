using System;

namespace ADMS.Apprentice.Core.Exceptions
{
    public class ExceptionDetailsAttribute : Attribute
    {
        public string ValidationRuleId { get; }
        public string Message { get; }

        public ExceptionDetailsAttribute(string validationRuleId, string message)
        {
            ValidationRuleId = validationRuleId;
            Message = message;
        }
    }
}