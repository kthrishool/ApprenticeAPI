
using System;

namespace ADMS.Apprentices.Core.TYIMS.Entities
{
    public record Registration
    {
        public int RegistrationId { get; set; }
        public string QualificationCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CurrentEndReasonCode { get; set; }
        public int TrainingContractId { get; set; }
    }
}