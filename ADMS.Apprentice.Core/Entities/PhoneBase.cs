using System;

namespace ADMS.Apprentice.Core.Entities
{
    public class PhoneBase
    {
        public string PhoneTypeCode { get; set; }
        public string PhoneNumber { get; set; }
        public Boolean PreferredPhoneFlag { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }
        public long AuditEventId { get; set; }
    }
}