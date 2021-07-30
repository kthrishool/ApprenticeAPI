using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared;

namespace ADMS.Apprentices.Core.Services
{
    public class PriorApprenticeshipCreator : IPriorApprenticeshipCreator
    {
        private readonly IRepository repository;

        private readonly IPriorApprenticeshipValidator priorApprenticeshipValidator;

        public PriorApprenticeshipCreator(IRepository repository,
            IPriorApprenticeshipValidator priorApprenticeshipValidator
        )
        {
            this.repository = repository;
            this.priorApprenticeshipValidator = priorApprenticeshipValidator;
        }

        public async Task<PriorApprenticeship> CreateAsync(int apprenticeId, ProfilePriorApprenticeshipMessage message)
        {
            PriorApprenticeship priorApprenticeship = new PriorApprenticeship
            {
                QualificationCode = message.QualificationCode.Sanitise(),
                QualificationDescription = message.QualificationDescription.Sanitise(),
                QualificationLevel = message.QualificationLevel.Sanitise(),
                QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise(),
                StateCode = message.StateCode,
                CountryCode = message.CountryCode,
                StartDate = message.StartDate,
                EndDate = message.EndDate
            };

            var profile = await GetProfileAndCheckQualificationForApprenticeships(apprenticeId, priorApprenticeship);

            profile.PriorApprenticeships.Add(priorApprenticeship);

            return priorApprenticeship;
        }

        public async Task<Profile> GetProfileAndCheckQualificationForApprenticeships(int apprenticeId, PriorApprenticeship priorApprenticeship)
        {
            // Need to throw an error if profile cannot be found as qualification validator doesn't support a profile with a null value.
            var profile = await repository.GetAsync<Profile>(apprenticeId, true);

            var exceptionBuilder = await priorApprenticeshipValidator.ValidateAsync(priorApprenticeship, profile);
            exceptionBuilder.ThrowAnyExceptions();

            return profile;
        }
    }
}