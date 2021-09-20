using System;
using ADMS.Services.Infrastructure.Model.Interface;
using Au.Gov.Infrastructure.EntityFramework.Entities;

namespace ADMS.Apprentices.Core.Entities
{
    public class ApprenticeUSI : IAuditedIdentifier, ITimestampEnabled
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }
        public string USI { get; set; }
        public bool ActiveFlag { get; set; }
        public string USIChangeReason { get; set; }
        public bool? USIVerifyFlag { get; set; }
        public bool? FirstNameMatchedFlag { get; set; }
        public bool? SurnameMatchedFlag { get; set; }
        public bool? DateOfBirthMatchedFlag { get; set; }

        public string USIStatus { get; set; }

        public virtual Profile Profile { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }

        public long AuditEventId { get; set; }
    }
}