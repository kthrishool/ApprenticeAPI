using System;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.ClaimSubmissions.Entities;
using ADMS.Apprentice.Core.ClaimSubmissions.Messages;
using Adms.Shared;

namespace ADMS.Apprentice.Core.ClaimSubmissions.Services
{
    public class ClaimSubmissionCreator : IClaimSubmissionCreator
    {
        private readonly IRepository repository;

        public ClaimSubmissionCreator(IRepository repository)
        {
            this.repository = repository;
        }

        public Task<ClaimSubmission> CreateAsync(ClaimSubmissionMessage message)
        {
            var claimSubmission = new ClaimSubmission
            {
                Type = message.Type.Value,
                Category = message.Category.Value,
                ApprenticeId = 111,
                ApprenticeName = "Joe Bloggs",
                EmployerId = 222,
                EmployerName = "Sally's Plumbing",
                NetworkProviderId = 333,
                NetworkProviderName = "Jobs R Us",
                SubmissionStatus = ClaimSubmissionStatus.AwaitingEmployerApproval,
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            };
            repository.Insert(claimSubmission);
            // doesn't need to be async just yet, but it will be once we start looking up TYIMS data etc
            return Task.FromResult(claimSubmission);
        }
    }
}