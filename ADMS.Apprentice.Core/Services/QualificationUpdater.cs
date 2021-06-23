﻿using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.Core.Helpers;
using System;
using System.Linq;
using Adms.Shared.Extensions;
using Adms.Shared;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.TYIMS.Entities;
using System.Collections.Generic;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public class QualificationUpdater : IQualificationUpdater
    {
        private readonly IQualificationValidator qualificationValidator;
        private readonly IRepository repository;
        private readonly ITYIMSRepository tyimsRepository;
        private readonly IExceptionFactory exceptionFactory;

        public QualificationUpdater(IRepository repository, ITYIMSRepository tyimsRepository, IExceptionFactory exceptionFactory, IQualificationValidator qualificationValidator)
        {
            this.repository = repository;
            this.tyimsRepository = tyimsRepository;
            this.qualificationValidator = qualificationValidator;
            this.exceptionFactory = exceptionFactory;
        }

        public async Task<Qualification> Update(int apprenticeId, int qualificationId, ProfileQualificationMessage message)
        {
            Task<Registration> registrationTask = null;
            Task<Profile> profileTask = repository.GetAsync<Profile>(apprenticeId, true);
            
            var tasks = new List<Task>() {profileTask};

            if (message.ApprenticeshipId != null) {
                registrationTask = tyimsRepository.GetRegistrationAsync(message.ApprenticeshipId.Value);
                tasks.Add(registrationTask);
            }
            
            /* Exceptions thrown will bubble automatically */
            await Task.WhenAll(tasks.ToArray());

            var profile = profileTask.Result;

            Qualification qualification = profile.Qualifications.SingleOrDefault(x => x.Id == qualificationId);
            if (qualification == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Qualification ", qualificationId.ToString());

            qualification.QualificationCode = message.QualificationCode.Sanitise();
            qualification.QualificationDescription = message.QualificationDescription.Sanitise();
            qualification.QualificationLevel = message.QualificationLevel.Sanitise();
            qualification.QualificationANZSCOCode = message.QualificationANZSCOCode.Sanitise();
            qualification.ApprenticeshipId = message.ApprenticeshipId;
            qualification.StartDate = message.StartDate;
            qualification.EndDate = message.EndDate;

            var exceptionBuilder = await qualificationValidator.ValidateAsync(qualification);
            if (registrationTask != null)
                exceptionBuilder.AddExceptions(qualificationValidator.ValidateAgainstApprenticeshipQualification(qualification, registrationTask.Result));
            exceptionBuilder.ThrowAnyExceptions();

            return qualification;
        }

    }
}