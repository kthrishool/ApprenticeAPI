using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record PriorQualificationDetailsMessage
    {
        [Display(Name = "Prior Qualifications")]
        public List<PriorQualificationMessage> Qualifications { get; init; }
    }
}