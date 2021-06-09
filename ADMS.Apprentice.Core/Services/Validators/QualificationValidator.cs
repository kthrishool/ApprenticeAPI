﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Adms.Shared.Extensions;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.Helpers;
using System;
using System.Globalization;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class QualificationValidator : IQualificationValidator
    {
        private readonly IExceptionFactory exceptionFactory;
        private readonly IReferenceDataValidator referenceDataValidator;

        public QualificationValidator(IExceptionFactory exceptionFactory, IReferenceDataValidator referenceDataValidator)     
        {            
            this.exceptionFactory = exceptionFactory;
            this.referenceDataValidator = referenceDataValidator;
        }

        public async Task<List<Qualification>> ValidateAsync(List<Qualification> qualifications)
        {
            var validQualifications = new List<Qualification>();

            foreach (Qualification qualification in qualifications)
            {
                await ValidateAsync(qualification);
                validQualifications.Add(qualification);                
            }

            return validQualifications;
        }

        public async Task<Qualification> ValidateAsync(Qualification qualification)
        {
            //check if mandatory fields presents
            
            if (qualification.QualificationCode.IsNullOrEmpty())
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

            //check if StartDate < endDate if startDate and EndDate not null
            if (qualification.StartDate?.Date > qualification.EndDate?.Date)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);

            //check if start date can not be less than apprentice DOB +12 years
            if ((qualification.StartDate != null && qualification.Profile != null))
            {
              if (qualification.StartDate.Value.Date < qualification.Profile.BirthDate.AddYears(+12))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);
            }
            //check if end date can not be less than apprentice DOB +12 years
            if (qualification.EndDate != null && qualification.Profile != null)
            {
                if (qualification.EndDate.Value.Date < qualification.Profile.BirthDate.AddYears(+12))
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);
            }
            //check if start date and end date can not be greater than today's date.
            if (qualification.StartDate != null && qualification.EndDate != null)
            {
                if (qualification.StartDate.Value.Date > DateTime.Today || qualification.EndDate.Value.Date > DateTime.Today)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidQualification);
            }

            await referenceDataValidator.ValidateAsync(qualification);

            return qualification;
        }

        public void CheckForDuplicates(List<Qualification> qualifications)
        {
            //check for duplicates based on Qcode
            if (qualifications.GroupBy(x => x.QualificationCode.ToUpper()).Any(g => g.Count() > 1))
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.DuplicateQualification);
        }
    }
}