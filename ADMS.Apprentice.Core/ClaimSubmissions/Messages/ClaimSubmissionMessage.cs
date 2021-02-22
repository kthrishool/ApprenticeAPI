using System.ComponentModel.DataAnnotations;
using ADMS.Apprentice.Core.ClaimSubmissions.Entities;

namespace ADMS.Apprentice.Core.ClaimSubmissions.Messages
{
    public record ClaimSubmissionMessage
    {
        [Required(ErrorMessage = "Type is required")]
        public ClaimType? Type { get; init; }

        [Required(ErrorMessage = "Category is required")]
        public ClaimCategory? Category { get; init; }

        [Required(ErrorMessage = "RegistrationId is required")]
        public int? RegistrationId { get; init; }
    }
}