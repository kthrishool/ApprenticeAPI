using System.ComponentModel;

namespace ADMS.Apprentices.Core.Messages
{
    public class ProfileSearchCriteria
    {
        [Description("Keyword to search on.")]
        public string Keyword { get; set; }

        public string StatusCode { get; set; }
    }
}