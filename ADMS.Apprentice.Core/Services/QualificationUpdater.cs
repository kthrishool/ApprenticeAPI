using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.Core.Helpers;
using System;
using System.Linq;
using Adms.Shared.Extensions;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public class QualificationUpdater : IQualificationUpdater
    {
        private readonly IQualificationValidator qualificationValidator;
        public QualificationUpdater(IQualificationValidator qualificationValidator)
        {
            this.qualificationValidator = qualificationValidator;
        }

        public async Task<Qualification> Update(Qualification qualification, ProfileQualificationMessage message)
        {
            qualification.QualificationCode = message.QualificationCode.Sanitise();
            qualification.QualificationDescription = message.QualificationDescription.Sanitise();
            qualification.QualificationLevel = message.QualificationLevel.Sanitise();
            qualification.QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise();
            qualification.StartDate = message.StartDate;
            qualification.EndDate = message.EndDate;

            var exceptionBuilder = await qualificationValidator.ValidateAsync(qualification);
            exceptionBuilder.ThrowAnyExceptions();

            return qualification;
        }

    }
}