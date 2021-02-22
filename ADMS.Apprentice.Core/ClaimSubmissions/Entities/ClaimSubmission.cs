using System;
using Adms.Shared;

namespace ADMS.Apprentice.Core.ClaimSubmissions.Entities
{
    public class ClaimSubmission: IAmAnAggregateRoot<int>
    {
        public int Id { get; set; }
        public ClaimSubmissionStatus SubmissionStatus { get; set; }
        public ClaimType Type { get; set; }
        public ClaimCategory Category { get; set; }
        public int ApprenticeId { get; set; }
        public string ApprenticeName { get; set; }
        public int EmployerId { get; set; }
        public string EmployerName { get; set; }
        public int NetworkProviderId { get; set; }
        public string NetworkProviderName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}