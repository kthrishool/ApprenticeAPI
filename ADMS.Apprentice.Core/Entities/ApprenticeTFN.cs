using Adms.Shared;
using ADMS.Services.Infrastructure.Model.Interface;
using System;

namespace ADMS.Apprentice.Core.Entities
{
    public class ApprenticeTFN : IAmAnAggregateRoot<int>, ITimestampEnabled, IAuditableIdentifier
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }
        public string TaxFileNumber { get; set; }
        public TFNStatus StatusCode { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusReasonCode { get; set; }
        public Guid MessageQueueCorrelationId { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }

        public long AuditEventId { get; set; }
    }

}