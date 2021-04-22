﻿using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Messages
{
    public record ProfileQualificationMessage
    {
        [Required(ErrorMessage = "Qualification code is required")]
        [MaxLength(10, ErrorMessage = "Qualification code cannot exceed 10 characters in length")]
        public string QualificationCode { get; set; }

        [MaxLength(50, ErrorMessage = "Qualification description cannot exceed 100 characters in length")]
        public string QualificationDescription { get; set; }
        
        [MaxLength(10, ErrorMessage = "Qualification level cannot exceed 10 characters in length")]
        public string QualificationLevel { get; set; }
        
        [MaxLength(10, ErrorMessage = "ANZSCO code cannot exceed 10 characters in length")]
        public string QualificationANZSCOCode { get; set; }

        public string StartMonth { get; set; }

        public int? StartYear { get; set; }

        public string EndMonth { get; set; }

        public int? EndYear { get; set; }        
    }
}