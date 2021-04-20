using System.ComponentModel;

namespace ADMS.Apprentice.Core.Messages
{
    public class ProfileSearchCriteria
    {
        [Description("Keyword to search on.")]
        public string Keyword { get; set; }

        public string StatusCode { get; set; }
    }
}