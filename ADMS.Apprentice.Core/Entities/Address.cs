using System;
using ADMS.Services.Infrastructure.Model.Interface;

namespace ADMS.Apprentice.Core.Entities
{
    public class Address : IAuditableIdentifier, ITimestampEnabled
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }

        public string AddressTypeCode { get; set; }

        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string StreetAddress3 { get; set; }
        public string Locality { get; set; }

        public string StateCode { get; set; }
        public string Postcode { get; set; }
        public string SingleLineAddress { get; set; }

        public string GeocodeType { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Confidence { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }
        public long AuditEventId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}