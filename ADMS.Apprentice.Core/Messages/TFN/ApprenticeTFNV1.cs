using System.ComponentModel.DataAnnotations;

// ReSharper disable InconsistentNaming

namespace ADMS.Apprentice.Core.Messages.TFN
{
    public record ApprenticeTFNV1 
    {
        [Required(ErrorMessage = "ApprenticeId is required")]
        public int ApprenticeId;

        [Required(ErrorMessage = "TFN is required")]
        public long TaxFileNumber;
    }
}