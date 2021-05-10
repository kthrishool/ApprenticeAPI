using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services.Validators;
using Adms.Shared;

namespace ADMS.Apprentice.Core.Services
{
    public class QualificationCreator : IQualificationCreator
    {
        private readonly IRepository repository;
        private readonly IQualificationValidator qualificationValidator;

        public QualificationCreator(IRepository repository,
            IQualificationValidator qualificationValidator)
        {
            this.repository = repository;
            this.qualificationValidator = qualificationValidator;
        }

        public async Task<Qualification> CreateAsync(ProfileQualificationMessage message)
        {
            Qualification qualification = new Qualification
            {
                QualificationCode = message.QualificationCode.Sanitise(),
                QualificationDescription = message.QualificationDescription.Sanitise(),
                QualificationLevel = message.QualificationLevel.Sanitise(),
                QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise(),
                StartMonth = message.StartMonth.SanitiseUpper(),
                StartYear = message.StartYear,
                EndMonth = message.EndMonth.SanitiseUpper(),
                EndYear = message.EndYear,
            };

            await qualificationValidator.ValidateAsync(qualification);           

            return qualification;
        }
    }
}