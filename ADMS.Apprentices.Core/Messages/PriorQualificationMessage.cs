using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record PriorQualificationMessage
    {
        [Required]
        [MaxLength(10, ErrorMessage = "Qualification code cannot exceed 10 characters in length")]
        public string QualificationCode { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "Qualification description cannot exceed 200 characters in length")]
        public string QualificationDescription { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Qualification level cannot exceed 10 characters in length")]
        public string QualificationLevel { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "ANZSCO code cannot exceed 10 characters in length")]
        public string QualificationANZSCOCode { get; set; }

        [Required]
        public bool? NotOnTrainingGovAu { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}