using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.HttpClients.ReferenceDataApi;
using Adms.Shared.Extensions;
using Adms.Shared.Exceptions;
using ADMS.Apprentices.Core.Helpers;
using System;
using System.Globalization;
using ADMS.Apprentices.Core.TYIMS.Entities;

namespace ADMS.Apprentices.Core.Services.Validators
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

        //public async Task<ValidationExceptionBuilder> ValidateAsync(List<Qualification> qualifications)
        //{
        //    var tasks = new List<Task<ValidationExceptionBuilder>>();

        //    foreach (Qualification qualification in qualifications)
        //    {
        //        tasks.Add(ValidateAsync(qualification));
        //    }

        //    return await tasks.WaitAndReturnExceptionBuilder();
        //}
        
        public async Task<ValidationExceptionBuilder> ValidateAsync(Qualification qualification, Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            //check if mandatory fields presents
            
            if (qualification.QualificationCode.IsNullOrEmpty()) {
                exceptionBuilder.AddException(ValidationExceptionType.MissingQualificationCode);
            }

            //check if StartDate < endDate if startDate and EndDate not null
            if (qualification.StartDate?.Date > qualification.EndDate?.Date)
                exceptionBuilder.AddException(ValidationExceptionType.DateMismatch);

            //check if start date can not be less than apprentice DOB +12 years
            if ((qualification.StartDate != null && profile != null))
            {
              if (qualification.StartDate.Value.Date < profile.BirthDate.AddYears(+12))
                    exceptionBuilder.AddException(ValidationExceptionType.DOBDateMismatch);
            }
            //check if end date can not be less than apprentice DOB +12 years
            if (qualification.EndDate != null && profile != null)
            {
                if (qualification.EndDate.Value.Date < profile.BirthDate.AddYears(+12))
                    exceptionBuilder.AddException(ValidationExceptionType.DOBDateMismatch);
            }
            //check if start date and end date can not be greater than today's date.
            if (qualification.StartDate != null && qualification.EndDate != null)
            {
                if (qualification.StartDate.Value.Date > DateTime.Today || qualification.EndDate.Value.Date > DateTime.Today)
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidDate);
            }
            
            if (qualification.ApprenticeshipId != null)
                if (qualification.StartDate == null || qualification.EndDate == null)
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidApprenticeshipQualification);

            exceptionBuilder.AddExceptions(await referenceDataValidator.ValidateAsync(qualification));

            return exceptionBuilder;
        }

        public ValidationExceptionBuilder CheckForDuplicates(List<Qualification> qualifications)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            //check for duplicates based on Qcode
            if (qualifications.GroupBy(x => x.QualificationCode.ToUpper()).Any(g => g.Count() > 1))
                exceptionBuilder.AddException(ValidationExceptionType.DuplicateQualification);
            return exceptionBuilder;
        }

        public ValidationExceptionBuilder ValidateAgainstApprenticeshipQualification(Qualification qualification, Registration registration)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            if(registration == null) {
                exceptionBuilder.AddException(ValidationExceptionType.QualificationApprenticeshipDoesNotExist);        
                return exceptionBuilder;
            }
            /* Should never happen is a bug if code occurs */
            // if(registration.RegistrationId != qualification.ApprenticeshipId) {
            //     exceptionBuilder.AddException(ValidationExceptionType.QualificationApprenticeshipDoesNotExist);        
            // }
            if(qualification.QualificationCode != registration.QualificationCode) {
                exceptionBuilder.AddException(ValidationExceptionType.QualificationApprenticeshipQualificationCodeDoesNotMatch);        
            }
            if(registration.EndDate == null) {
                exceptionBuilder.AddException(ValidationExceptionType.QualificationApprenticeshipIsNotComplete);
            }
            if(registration.CurrentEndReasonCode != "CMPS") {
                exceptionBuilder.AddException(ValidationExceptionType.QualificationApprenticeshipIsNotComplete);
            }
            return exceptionBuilder;
        }
    }
}