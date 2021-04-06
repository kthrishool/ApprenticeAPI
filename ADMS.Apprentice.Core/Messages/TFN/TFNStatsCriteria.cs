using System.ComponentModel;

namespace ADMS.Apprentice.Core.Messages.TFN
{
    public class TFNStatsCriteria
    {
        [Description("Keyword to search on.")]
        public string Keyword { get; set; }

        public string StatusCode { get; set; }
    }
}
