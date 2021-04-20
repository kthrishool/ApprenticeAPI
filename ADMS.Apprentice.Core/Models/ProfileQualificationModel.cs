using ADMS.Apprentice.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADMS.Apprentice.Core.Models
{
    public record ProfileQualificationModel
    {
        public int QualificationId { get; }
        public string QualificationCode { get; }
        public string QualificationDescription { get; }
        public string StartMonth { get; }
        public string StartYear { get; }
        public string EndMonth { get; }
        public string EndYear { get; }

        public DateTime? CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime? UpdatedOn { get; }
        public string UpdatedBy { get; }

        public ProfileQualificationModel() { }

        public ProfileQualificationModel(Qualification qualification)
        {
            QualificationId = qualification.Id;
            QualificationCode = qualification.QualificationCode;
            QualificationDescription = qualification.QualificationDescription;
            StartMonth = qualification.StartDate.ToString("MMM").ToUpper();
            StartYear = qualification.StartDate.Year.ToString();
            EndMonth = qualification.EndDate.ToString("MMM").ToUpper();
            EndYear = qualification.EndDate.Year.ToString();

            CreatedOn = qualification.CreatedOn;
            CreatedBy = qualification.CreatedBy;
            UpdatedOn = qualification.UpdatedOn;
            UpdatedBy = qualification.UpdatedBy;
        }
    }
}