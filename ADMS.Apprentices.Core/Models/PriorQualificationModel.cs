using System;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Models
{
    public record PriorQualificationModel
    {
        public int Id { get; }
        public string QualificationCode { get; }
        public string QualificationDescription { get; }
        public string QualificationLevel { get; }
        public string QualificationANZSCOCode { get; }
        public string QualificationManualReasonCode { get; }
        public DateTime? StartDate { get; }
        public DateTime? EndDate { get; }

        public DateTime? CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime? UpdatedOn { get; }
        public string UpdatedBy { get; }

        public PriorQualificationModel(PriorQualification qualification)
        {
            Id = qualification.Id;
            QualificationCode = qualification.QualificationCode;
            QualificationDescription = qualification.QualificationDescription;
            QualificationLevel = qualification.QualificationLevel;
            QualificationANZSCOCode = qualification.QualificationANZSCOCode;
            QualificationManualReasonCode = qualification.QualificationManualReasonCode;
            StartDate = qualification.StartDate;
            EndDate = qualification.EndDate;            
            CreatedOn = qualification.CreatedOn;
            CreatedBy = qualification.CreatedBy;
            UpdatedOn = qualification.UpdatedOn;
            UpdatedBy = qualification.UpdatedBy;
        }
    }
}