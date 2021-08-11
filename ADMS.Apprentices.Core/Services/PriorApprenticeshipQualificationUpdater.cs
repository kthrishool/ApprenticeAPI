using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared;
using Adms.Shared.Attributes;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public class PriorApprenticeshipQualificationUpdater : IPriorApprenticeshipQualificationUpdater
    {
        private readonly IPriorApprenticeshipQualificationValidator priorApprenticeshipValidator;
        private readonly IRepository repository;
        private readonly ITYIMSRepository tyimsRepository;

        public PriorApprenticeshipQualificationUpdater(IRepository repository, ITYIMSRepository tyimsRepository,
            IPriorApprenticeshipQualificationValidator priorApprenticeshipValidator)
        {
            this.repository = repository;
            this.tyimsRepository = tyimsRepository;
            this.priorApprenticeshipValidator = priorApprenticeshipValidator;
        }

        public async Task<PriorApprenticeshipQualification> Update(int apprenticeId, int qualificationId, PriorApprenticeshipQualificationMessage message, Profile profile)
        {
            PriorApprenticeshipQualification priorApprenticeship = profile.PriorApprenticeshipQualifications.SingleOrDefault(x => x.Id == qualificationId);
            if (priorApprenticeship == null)
                throw AdmsNotFoundException.Create("Apprentice Qualification ", qualificationId.ToString());

            priorApprenticeship.QualificationCode = message.QualificationCode.Sanitise();
            priorApprenticeship.QualificationDescription = message.QualificationDescription.Sanitise();
            priorApprenticeship.QualificationLevel = message.QualificationLevel.Sanitise();
            priorApprenticeship.QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise();
            priorApprenticeship.StartDate = message.StartDate;
            priorApprenticeship.EndDate = message.EndDate;
            priorApprenticeship.CountryCode = message.CountryCode;
            priorApprenticeship.StateCode = message.StateCode;

            var exceptionBuilder = await priorApprenticeshipValidator.ValidateAsync(priorApprenticeship, profile);

            exceptionBuilder.ThrowAnyExceptions();

            return priorApprenticeship;
        }
    }
}