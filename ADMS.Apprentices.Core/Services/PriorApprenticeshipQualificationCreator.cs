using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared;

// ReSharper disable PossibleInvalidOperationException

namespace ADMS.Apprentices.Core.Services
{
    public class PriorApprenticeshipQualificationCreator : IPriorApprenticeshipQualificationCreator
    {
        private readonly IRepository repository;

        private readonly IPriorApprenticeshipQualificationValidator priorApprenticeshipValidator;

        public PriorApprenticeshipQualificationCreator(IRepository repository,
            IPriorApprenticeshipQualificationValidator priorApprenticeshipValidator
        )
        {
            this.repository = repository;
            this.priorApprenticeshipValidator = priorApprenticeshipValidator;
        }

        public async Task<PriorApprenticeshipQualification> CreateAsync(int apprenticeId, PriorApprenticeshipQualificationMessage message)
        {
            PriorApprenticeshipQualification priorApprenticeship = new()
            {
                EmployerName = message.EmployerName.Sanitise(),
                QualificationCode = message.QualificationCode.Sanitise(),
                QualificationDescription = message.QualificationDescription.Sanitise(),
                QualificationLevel = message.QualificationLevel.Sanitise(),
                QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise(),
                QualificationManualReasonCode = message.QualificationManualReasonCode.Sanitise(),
                StateCode = message.StateCode,
                CountryCode = message.CountryCode,
                StartDate = message.StartDate,
                ApprenticeshipReference = message.ApprenticeshipReference
            };

            var profile = await GetProfileAndCheckQualificationForApprenticeships(apprenticeId, priorApprenticeship);

            profile.PriorApprenticeshipQualifications.Add(priorApprenticeship);

            return priorApprenticeship;
        }

        public async Task<Profile> GetProfileAndCheckQualificationForApprenticeships(int apprenticeId, PriorApprenticeshipQualification priorApprenticeship)
        {
            // Need to throw an error if profile cannot be found as qualification validator doesn't support a profile with a null value.
            var profile = await repository.GetAsync<Profile>(apprenticeId, true);

            var exceptionBuilder = await priorApprenticeshipValidator.ValidateAsync(priorApprenticeship, profile);
            exceptionBuilder.ThrowAnyExceptions();

            return profile;
        }
    }
}