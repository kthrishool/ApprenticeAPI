using System;

namespace ADMS.Apprentice.Core.Entities
{
    public class ApprenticeUSI
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }
        public string USI { get; set; }
        public Boolean ActiveFlag { get; set; }
        public string USIChangeReason { get; set; }
        public Boolean USIVerifyFlag { get; set; }
        public Boolean FirstNameMatchedFlag { get; set; }
        public Boolean SurnameMatchedFlag { get; set; }
        public Boolean DateOfBirthMatchedFlag { get; set; }

        public string USIStatus { get; set; }

        public virtual Profile Profile { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }

        public long AuditEventId { get; set; }
    }
}