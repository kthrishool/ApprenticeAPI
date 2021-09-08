using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ADMS.Apprentices.Core.Helpers
{
    // Code will be review and then Test cases added 
    [ExcludeFromCodeCoverage]
    public class ADMSEmailAddressAttribute : ValidationAttribute
    {
        public String Code { get; set; }
        public Boolean IsRequired { get; set; }

        public ADMSEmailAddressAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
            }
            return true;
        }
    }
}