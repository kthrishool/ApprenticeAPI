﻿using System;
using ADMS.Services.Infrastructure.Model.Interface;

namespace ADMS.Apprentice.Core.Entities
{
    public class Guardian : IAuditableIdentifier, ITimestampEnabled
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }

        public string EmailAddress { get; set; }

        public string LandLine { get; set; }

        public string Mobile { get; set; }
        public string WorkPhoneNumber { get; set; }

        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }

        public string StreetAddress3 { get; set; }
        public string Locality { get; set; }

        public string StateCode { get; set; }
        public string Postcode { get; set; }
        public string SingleLineAddress { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }
        public long AuditEventId { get; set; }
    }
}