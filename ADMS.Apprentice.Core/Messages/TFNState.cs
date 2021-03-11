using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Messages
{
    public record TFNState
    {
        /// <summary>
        /// Status of the TFN verification.
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        public TFNStatus Status { get; init; }
    }
}