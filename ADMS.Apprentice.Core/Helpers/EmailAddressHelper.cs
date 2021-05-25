using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;

namespace ADMS.Apprentice.Core.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class EmailAddressHelper
    {
        public static bool EmailValidation(string emailAddress)
        {
            if (emailAddress.IsNullOrEmpty())
                return true;
            if (!(new EmailAddressAttribute().IsValid(emailAddress)))
                return false;

            // check domain name in Email
            if (emailAddress != null)
            {
                var domainName = emailAddress.Substring(emailAddress.LastIndexOf('@') + 1);
                if (domainName.IndexOf('.') < 1)
                    return false;
                else if (emailAddress.IndexOf("..", StringComparison.Ordinal) >= 0)
                    return false;
            }
            return true;
        }
    }
}
