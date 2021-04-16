﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Adms.Shared;
using ADMS.Services.Infrastructure.Model.Interface;

namespace ADMS.Apprentice.Core.Entities
{
    public class Qualification : IAuditableIdentifier, ITimestampEnabled
    {
        public int Id { get; set; }
        public int ApprenticeId { get; set; }

        public string QualificationCode { get; set; }
        public string QualificationDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }        
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] Version { get; set; }
        public long AuditEventId { get; set; }

        //Not mapped fields
        public string StartMonth;
        public string StartYear;
        public string EndMonth; 
        public string EndYear;

        public virtual Profile Profile { get; set; }
    }
}