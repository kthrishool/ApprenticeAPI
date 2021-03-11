using System.ComponentModel.DataAnnotations;
using System.Threading;
using ADMS.Apprentice.Core.Entities;

// ReSharper disable InconsistentNaming

namespace ADMS.Apprentice.Core.Messages
{
    public record TFNV1 : TFNState
    {
        [Required(ErrorMessage = "ApprenticeId is required")]
        public int ApprenticeId { get; set;}

        [Required(ErrorMessage = "TFN is required")]
        public string TaxFileNumber { get; set;}

        
    }
}