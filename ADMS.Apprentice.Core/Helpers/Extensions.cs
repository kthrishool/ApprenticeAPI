using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Helpers
{
    public static class StringExtensions
    {
        //TODO: need to move to AMDS shared
        public static string Sanitise(this String str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) ? null : str.Trim();
        }

        public static string SanitiseUpper(this String str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) ? null : str.Trim().ToUpper();
        }
    }
}
