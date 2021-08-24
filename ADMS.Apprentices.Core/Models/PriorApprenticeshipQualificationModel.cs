using System;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Models
{
    public record PriorApprenticeshipQualificationModel
    {
        public int Id { get; }
        public string EmployerName { get; }
        public string QualificationCode { get; }
        public string QualificationDescription { get; }
        public string QualificationLevel { get; }
        public string QualificationANZSCOCode { get; }
        public string QualificationManualReasonCode { get; }
        public DateTime? StartDate { get; }
        public string CountryCode { get; }

        public string StateCode { get; }
        public string ApprenticeshipReference { get; }

        public DateTime? CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime? UpdatedOn { get; }
        public string UpdatedBy { get; }
        public byte[] Version { get; }

        public PriorApprenticeshipQualificationModel(PriorApprenticeshipQualification priorApprenticeship)
        {
            Id = priorApprenticeship.Id;
            EmployerName = priorApprenticeship.EmployerName;
            QualificationCode = priorApprenticeship.QualificationCode;
            QualificationDescription = priorApprenticeship.QualificationDescription;
            QualificationLevel = priorApprenticeship.QualificationLevel;
            QualificationANZSCOCode = priorApprenticeship.QualificationANZSCOCode;
            QualificationManualReasonCode = priorApprenticeship.QualificationManualReasonCode;
            StartDate = priorApprenticeship.StartDate;
            ApprenticeshipReference = priorApprenticeship.ApprenticeshipReference;
            CountryCode = priorApprenticeship.CountryCode;
            StateCode = priorApprenticeship.StateCode;
            CreatedOn = priorApprenticeship.CreatedOn;
            CreatedBy = priorApprenticeship.CreatedBy;
            UpdatedOn = priorApprenticeship.UpdatedOn;
            UpdatedBy = priorApprenticeship.UpdatedBy;
            Version = priorApprenticeship.Version;
        }
    }
}