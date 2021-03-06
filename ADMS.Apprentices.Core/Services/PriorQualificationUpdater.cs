using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared;
using Adms.Shared.Attributes;
using Adms.Shared.Exceptions;

// ReSharper disable PossibleInvalidOperationException

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public class PriorQualificationUpdater : IPriorQualificationUpdater
    {
        private readonly IQualificationValidator qualificationValidator;
        private readonly IRepository repository;

        public PriorQualificationUpdater(IRepository repository, IQualificationValidator qualificationValidator)
        {
            this.repository = repository;
            this.qualificationValidator = qualificationValidator;
        }

        public async Task<PriorQualification> Update(int apprenticeId, int qualificationId, PriorQualificationMessage message)
        {
            // Need to throw an error if profile cannot be found as qualification validator doesn't support a profile with a null value.
            var profile = await repository.GetAsync<Profile>(apprenticeId, true);

            PriorQualification priorQualification = profile.PriorQualifications.SingleOrDefault(x => x.Id == qualificationId);
            if (priorQualification == null)
                throw AdmsNotFoundException.Create("Apprentice Qualification ", qualificationId.ToString());

            priorQualification.QualificationCode = message.QualificationCode.Sanitise();
            priorQualification.QualificationDescription = message.QualificationDescription.Sanitise();
            priorQualification.QualificationLevel = message.QualificationLevel.Sanitise();
            priorQualification.QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise();
            priorQualification.QualificationManualReasonCode = message.QualificationManualReasonCode.Sanitise();
            priorQualification.StartDate = message.StartDate;
            priorQualification.EndDate = message.EndDate;

            var exceptionBuilder = qualificationValidator.ValidatePriorQualification(priorQualification, profile);

            exceptionBuilder.ThrowAnyExceptions();

            return priorQualification;
        }
    }
}