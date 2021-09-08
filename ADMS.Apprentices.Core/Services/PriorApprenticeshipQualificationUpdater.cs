using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared.Attributes;
using Adms.Shared.Exceptions;

// ReSharper disable PossibleInvalidOperationException

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public class PriorApprenticeshipQualificationUpdater : IPriorApprenticeshipQualificationUpdater
    {
        private const string australia = "1101";
        private readonly IPriorApprenticeshipQualificationValidator priorApprenticeshipValidator;

        public PriorApprenticeshipQualificationUpdater(IPriorApprenticeshipQualificationValidator priorApprenticeshipValidator)
        {
            this.priorApprenticeshipValidator = priorApprenticeshipValidator;
        }

        public async Task<PriorApprenticeshipQualification> Update(int apprenticeId, int qualificationId, PriorApprenticeshipQualificationMessage message, Profile profile)
        {
            PriorApprenticeshipQualification priorApprenticeship = profile.PriorApprenticeshipQualifications.SingleOrDefault(x => x.Id == qualificationId);
            if (priorApprenticeship == null)
                throw AdmsNotFoundException.Create("Apprentice Qualification ", qualificationId.ToString());

            priorApprenticeship.EmployerName = message.EmployerName.Sanitise();
            priorApprenticeship.QualificationCode = message.QualificationCode.Sanitise();
            priorApprenticeship.QualificationDescription = message.QualificationDescription.Sanitise();
            priorApprenticeship.QualificationLevel = message.QualificationLevel.Sanitise();
            priorApprenticeship.QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise();
            priorApprenticeship.QualificationManualReasonCode = message.QualificationManualReasonCode.Sanitise();
            priorApprenticeship.StartDate = message.StartDate;
            priorApprenticeship.CountryCode = message.CountryCode;
            priorApprenticeship.StateCode = priorApprenticeship.CountryCode == australia ? message.StateCode : null;
            priorApprenticeship.ApprenticeshipReference = message.ApprenticeshipReference;

            var exceptionBuilder = await priorApprenticeshipValidator.ValidateAsync(priorApprenticeship, profile);

            exceptionBuilder.ThrowAnyExceptions();

            return priorApprenticeship;
        }
    }
}