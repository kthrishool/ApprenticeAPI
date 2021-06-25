using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record QualificationDetailsMessage
    {
        [Display(Name = "Qualifications")]
        public List<ProfileQualificationMessage> Qualifications { get; init; }
    }
}