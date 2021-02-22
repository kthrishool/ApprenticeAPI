using System;
using ADMS.Apprentice.Core.ClaimSubmissions.Entities;

namespace ADMS.Apprentice.Core.ClaimSubmissions.Models
{
    public record ClaimSubmissionListModel
    {
        public int Id { get; }
        public ClaimSubmissionStatus SubmissionStatus { get; }
        public ClaimType Type { get; }
        public ClaimCategory Category { get; }
        public int ApprenticeId { get; }
        public string ApprenticeName { get; }
        public int EmployerId { get; }
        public string EmployerName { get; }
        public int NetworkProviderId { get; }
        public string NetworkProviderName { get; }
        public DateTime CreatedDate { get; }
        public DateTime LastModifiedDate { get; }

        public ClaimSubmissionListModel(ClaimSubmission claimSubmission)
        {
            Id = claimSubmission.Id;
            SubmissionStatus = claimSubmission.SubmissionStatus;
            Type = claimSubmission.Type;
            Category = claimSubmission.Category;
            ApprenticeId = claimSubmission.ApprenticeId;
            ApprenticeName = claimSubmission.ApprenticeName;
            EmployerId = claimSubmission.EmployerId;
            EmployerName = claimSubmission.EmployerName;
            NetworkProviderId = claimSubmission.NetworkProviderId;
            NetworkProviderName = claimSubmission.NetworkProviderName;
            CreatedDate = claimSubmission.CreatedDate;
            LastModifiedDate = claimSubmission.LastModifiedDate;
        }

    }
}