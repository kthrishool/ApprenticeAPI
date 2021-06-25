using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared;
using ADMS.Apprentices.Core.TYIMS.Entities;
using System.Collections.Generic;

namespace ADMS.Apprentices.Core.Services
{
    public class QualificationCreator : IQualificationCreator
    {
        private readonly IRepository repository;
        private readonly IQualificationValidator qualificationValidator;
        private readonly ITYIMSRepository tyimsRepository;

        public QualificationCreator(IRepository repository,
            IQualificationValidator qualificationValidator,
            ITYIMSRepository tyimsRepository)
        {
            this.repository = repository;
            this.qualificationValidator = qualificationValidator;
            this.tyimsRepository = tyimsRepository;
        }

        public async Task<Qualification> CreateAsync(int apprenticeId, ProfileQualificationMessage message)
        {

            Qualification qualification = new Qualification
            {
                QualificationCode = message.QualificationCode.Sanitise(),
                QualificationDescription = message.QualificationDescription.Sanitise(),
                QualificationLevel = message.QualificationLevel.Sanitise(),
                QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise(),
                ApprenticeshipId = message.ApprenticeshipId,
                StartDate = message.StartDate,
                EndDate = message.EndDate
            };

            var profile = await GetProfileAndCheckQualificationForApprenticeships(apprenticeId, qualification);

            profile.Qualifications.Add(qualification);


            return qualification;
        }
        
        public async Task<Profile> GetProfileAndCheckQualificationForApprenticeships(int apprenticeId, Qualification qualification)
        {
            var registrationId = qualification.ApprenticeshipId;
            
            // Repository will throw an error if profile cannot be found.
            Task<Profile> profileTask = repository.GetAsync<Profile>(apprenticeId, true);
            Task<ValidationExceptionBuilder> exceptionBuilderTask = qualificationValidator.ValidateAsync(qualification);           
            var tasks = new List<Task>() { profileTask, exceptionBuilderTask };

            Task<Registration> registrationTask = null;
            if(registrationId != null) {
                registrationTask = tyimsRepository.GetRegistrationAsync(registrationId.Value);
                tasks.Add(registrationTask);
            }

            // WaitAll will throw any exceptions so we don't need to look for them.
            await Task.WhenAll(tasks.ToArray());
            
            var exceptionBuilder = exceptionBuilderTask.Result;
            
            if(registrationTask != null) /* If registration could not be found then the validator will throw an exception for us */
                exceptionBuilder.AddExceptions(qualificationValidator.ValidateAgainstApprenticeshipQualification(qualification, registrationTask.Result));
            
            exceptionBuilder.ThrowAnyExceptions();
            
            return profileTask.Result;
        }
    }
}