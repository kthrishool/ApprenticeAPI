using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record PriorQualificationMessage
    {
        [Required]
        [MaxLength(10, ErrorMessage = "Qualification code cannot exceed 10 characters in length")]
        public string QualificationCode { get; init; }

        [Required]
        [MaxLength(200, ErrorMessage = "Qualification description cannot exceed 200 characters in length")]
        public string QualificationDescription { get; init; }

        [Required]
        [MaxLength(10, ErrorMessage = "Qualification level cannot exceed 10 characters in length")]
        public string QualificationLevel { get; init; }

        [Required]
        [MaxLength(10, ErrorMessage = "ANZSCO code cannot exceed 10 characters in length")]
        public string QualificationANZSCOCode { get; init; }

        [Adms.Shared.Attributes.MaxLength(10, "Qualification manual reason code")]
        public string QualificationManualReasonCode { get; init; }

        public DateTime? StartDate { get; init; }

        public DateTime? EndDate { get; init; }
    }
}