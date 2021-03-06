using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record SchoolDetailsMessage
    {
        [Display(Name = "HighestSchoolLevelCode")]
        [MaxLength(10, ErrorMessage = "Highest School Level Code Exceeds 10 Characters")]
        public string HighestSchoolLevelCode { get; init; }

        [MaxLength(10, ErrorMessage = "Left School Month code Exceeds 10 Characters")]
        public string LeftSchoolMonthCode { get; init; }
        
        public int? LeftSchoolYear { get; init; }
    }
}