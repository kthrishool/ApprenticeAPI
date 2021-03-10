using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Messages
{
    public class TfnCreateMessage : TfnUpdateMessage
    {
        [Required(ErrorMessage = "ApprenticeId is required")]
        public int ApprenticeId { get; set;}

        [Required(ErrorMessage = "TFN is required")]
        public string TFN { get; set;}
    }
}