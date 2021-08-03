using System;
using ADMS.Services.Infrastructure.Model.Interface;

namespace ADMS.Apprentices.Core.Entities
{
    public class PriorApprenticeship : IAuditableIdentifier, ITimestampEnabled, IQualificationAttributes
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }

        public string QualificationCode { get; set; }
        public string QualificationDescription { get; set; }
        public string QualificationLevel { get; set; }
        public string QualificationANZSCOCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string StateCode { get; set; }

        public string CountryCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }
        public long AuditEventId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}