using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record PriorQualificationMessage
    {
        [Required(ErrorMessage = "Qualification code is required")]
        [MaxLength(10, ErrorMessage = "Qualification code cannot exceed 10 characters in length")]
        public string QualificationCode { get; set; }

        [MaxLength(200, ErrorMessage = "Qualification description cannot exceed 200 characters in length")]
        public string QualificationDescription { get; set; }

        [MaxLength(10, ErrorMessage = "Qualification level cannot exceed 10 characters in length")]
        public string QualificationLevel { get; set; }

        [MaxLength(10, ErrorMessage = "ANZSCO code cannot exceed 10 characters in length")]
        public string QualificationANZSCOCode { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}