using System.Collections.Generic;
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
        private readonly IValidatorExceptionBuilderFactory exceptionBuilderFactory;
        private readonly IReferenceDataValidator referenceDataValidator;

        public QualificationValidator(IValidatorExceptionBuilderFactory exceptionBuilderFactory, IReferenceDataValidator referenceDataValidator)     
        {            
            this.exceptionBuilderFactory = exceptionBuilderFactory;
            this.referenceDataValidator = referenceDataValidator;
        }

        public async Task<IValidatorExceptionBuilder> ValidateAsync(List<Qualification> qualifications)
        {
            var tasks = new List<Task<IValidatorExceptionBuilder>>();

            foreach (Qualification qualification in qualifications)
            {
                tasks.Add(ValidateAsync(qualification));
            }

            return await tasks.WaitAndReturnExceptionBuilder();
        }
        
        public async Task<IValidatorExceptionBuilder> ValidateAsync(Qualification qualification)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            //check if mandatory fields presents
            
            if (qualification.QualificationCode.IsNullOrEmpty()) {
                exceptionBuilder.Add(ValidationExceptionType.InvalidQualification);
            }

            //check if StartDate < endDate if startDate and EndDate not null
            if (qualification.StartDate?.Date > qualification.EndDate?.Date)
                exceptionBuilder.Add(ValidationExceptionType.InvalidQualification);

            //check if start date can not be less than apprentice DOB +12 years
            if ((qualification.StartDate != null && qualification.Profile != null))
            {
              if (qualification.StartDate.Value.Date < qualification.Profile.BirthDate.AddYears(+12))
                    exceptionBuilder.Add(ValidationExceptionType.InvalidQualification);
            }
            //check if end date can not be less than apprentice DOB +12 years
            if (qualification.EndDate != null && qualification.Profile != null)
            {
                if (qualification.EndDate.Value.Date < qualification.Profile.BirthDate.AddYears(+12))
                    exceptionBuilder.Add(ValidationExceptionType.InvalidQualification);
            }
            //check if start date and end date can not be greater than today's date.
            if (qualification.StartDate != null && qualification.EndDate != null)
            {
                if (qualification.StartDate.Value.Date > DateTime.Today || qualification.EndDate.Value.Date > DateTime.Today)
                    exceptionBuilder.Add(ValidationExceptionType.InvalidQualification);
            }

            exceptionBuilder.AddExceptions(await referenceDataValidator.ValidateAsync(qualification));

            return exceptionBuilder;
        }

        public IValidatorExceptionBuilder CheckForDuplicates(List<Qualification> qualifications)
        {
            var exceptionBuilder = exceptionBuilderFactory.CreateExceptionBuilder();
            //check for duplicates based on Qcode
            if (qualifications.GroupBy(x => x.QualificationCode.ToUpper()).Any(g => g.Count() > 1))
                exceptionBuilder.Add(ValidationExceptionType.DuplicateQualification);
            return exceptionBuilder;
        }
    }
}