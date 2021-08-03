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
    public class QualificationUpdater : IQualificationUpdater
    {
        private readonly IQualificationValidator qualificationValidator;
        private readonly IRepository repository;
        private readonly ITYIMSRepository tyimsRepository;

        public QualificationUpdater(IRepository repository, ITYIMSRepository tyimsRepository,
            IQualificationValidator qualificationValidator)
        {
            this.repository = repository;
            this.tyimsRepository = tyimsRepository;
            this.qualificationValidator = qualificationValidator;
        }

        public async Task<Qualification> Update(int apprenticeId, int qualificationId, ProfileQualificationMessage message)
        {
            // Need to throw an error if profile cannot be found as qualification validator doesn't support a profile with a null value.
            var profile = await repository.GetAsync<Profile>(apprenticeId, true);

            Qualification qualification = profile.Qualifications.SingleOrDefault(x => x.Id == qualificationId);
            if (qualification == null)
                throw AdmsNotFoundException.Create("Apprentice Qualification ", qualificationId.ToString());

            qualification.QualificationCode = message.QualificationCode.Sanitise();
            qualification.QualificationDescription = message.QualificationDescription.Sanitise();
            qualification.QualificationLevel = message.QualificationLevel.Sanitise();
            qualification.QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise();
            qualification.StartDate = message.StartDate;
            qualification.EndDate = message.EndDate;

            var exceptionBuilder = await qualificationValidator.ValidateAsync(qualification, profile);

            exceptionBuilder.ThrowAnyExceptions();

            return qualification;
        }
    }
}