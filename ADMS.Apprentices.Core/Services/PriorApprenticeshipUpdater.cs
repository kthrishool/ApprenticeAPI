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
    public class PriorApprenticeshipUpdater : IPriorApprenticeshipUpdater
    {
        private readonly IPriorApprenticeshipValidator priorApprenticeshipValidator;
        private readonly IRepository repository;
        private readonly ITYIMSRepository tyimsRepository;

        public PriorApprenticeshipUpdater(IRepository repository, ITYIMSRepository tyimsRepository,
            IPriorApprenticeshipValidator priorApprenticeshipValidator)
        {
            this.repository = repository;
            this.tyimsRepository = tyimsRepository;
            this.priorApprenticeshipValidator = priorApprenticeshipValidator;
        }

        public async Task<PriorApprenticeship> Update(int apprenticeId, int qualificationId, ProfilePriorApprenticeshipMessage message, Profile profile)
        {
            PriorApprenticeship priorApprenticeship = profile.PriorApprenticeships.SingleOrDefault(x => x.Id == qualificationId);
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