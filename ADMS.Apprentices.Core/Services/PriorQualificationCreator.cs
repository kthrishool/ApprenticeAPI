using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared;

// ReSharper disable PossibleInvalidOperationException

namespace ADMS.Apprentices.Core.Services
{
    public class PriorQualificationCreator : IPriorQualificationCreator
    {
        private readonly IRepository repository;
        private readonly IQualificationValidator qualificationValidator;

        public PriorQualificationCreator(IRepository repository, IQualificationValidator qualificationValidator)
        {
            this.repository = repository;
            this.qualificationValidator = qualificationValidator;
        }

        public async Task<PriorQualification> CreateAsync(int apprenticeId, PriorQualificationMessage message)
        {
            PriorQualification qualification = new PriorQualification
            {
                QualificationCode = message.QualificationCode.Sanitise(),
                QualificationDescription = message.QualificationDescription.Sanitise(),
                QualificationLevel = message.QualificationLevel.Sanitise(),
                QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise(),
                NotOnTrainingGovAu = message.NotOnTrainingGovAu.Value,
                StartDate = message.StartDate,
                EndDate = message.EndDate
            };

            var profile = await GetProfileAndCheckQualificationForApprenticeships(apprenticeId, qualification);
            profile.PriorQualifications.Add(qualification);

            return qualification;
        }

        public async Task<Profile> GetProfileAndCheckQualificationForApprenticeships(int apprenticeId, PriorQualification qualification)
        {
            // Need to throw an error if profile cannot be found as qualification validator doesn't support a profile with a null value.
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);

            // WaitAll will throw any exceptions so we don't need to look for them.
            var exceptionBuilder = await qualificationValidator.ValidatePriorQualificationAsync(qualification, profile);


            exceptionBuilder.ThrowAnyExceptions();

            return profile;
        }
    }
}