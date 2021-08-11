using System;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Models
{
    public record PriorApprenticeshipQualificationModel
    {
        public int PriorApprenticeshipId { get; }
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

        public byte[] Version { get; }

        public PriorApprenticeshipQualificationModel(PriorApprenticeshipQualification apprenticeship)
        {
            PriorApprenticeshipId = apprenticeship.Id;
            QualificationCode = apprenticeship.QualificationCode;
            QualificationDescription = apprenticeship.QualificationDescription;
            QualificationLevel = apprenticeship.QualificationLevel;
            QualificationANZSCOCode = apprenticeship.QualificationANZSCOCode;
            StartDate = apprenticeship.StartDate;
            EndDate = apprenticeship.EndDate;

            CreatedOn = apprenticeship.CreatedOn;
            CreatedBy = apprenticeship.CreatedBy;
            UpdatedOn = apprenticeship.UpdatedOn;
            UpdatedBy = apprenticeship.UpdatedBy;
            Version = apprenticeship.Version;
        }
    }
}