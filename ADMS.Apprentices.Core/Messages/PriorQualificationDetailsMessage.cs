using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentices.Core.Messages
{
    public record PriorQualificationDetailsMessage
    {
        [Display(Name = "Qualifications")]
        public List<ProfilePriorQualificationMessage> Qualifications { get; init; }
    }
}