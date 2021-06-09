using System;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Models
{
    public record ProfileQualificationModel
    {
        public int QualificationId { get; }
        public string QualificationCode { get; }
        public string QualificationDescription { get; }
        public string QualificationLevel { get; }
        public string QualificationANZSCOCode { get; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime? UpdatedOn { get; }
        public string UpdatedBy { get; }

        public ProfileQualificationModel(Qualification qualification)
        {
            QualificationId = qualification.Id;
            QualificationCode = qualification.QualificationCode;
            QualificationDescription = qualification.QualificationDescription;
            QualificationLevel = qualification.QualificationLevel;
            QualificationANZSCOCode = qualification.QualificationANZSCOCode;
            StartDate = qualification.StartDate;
            EndDate = qualification.EndDate;
            CreatedOn = qualification.CreatedOn;
            CreatedBy = qualification.CreatedBy;
            UpdatedOn = qualification.UpdatedOn;
            UpdatedBy = qualification.UpdatedBy;
        }
    }
}