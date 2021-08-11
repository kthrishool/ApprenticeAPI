using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Extensions;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class PriorQualificationValidator : IQualificationValidator
    {
        private readonly IReferenceDataValidator referenceDataValidator;

        public PriorQualificationValidator(IReferenceDataValidator referenceDataValidator)
        {
            this.referenceDataValidator = referenceDataValidator;
        }


        public async Task<ValidationExceptionBuilder> ValidateAsync([NotNull] IQualificationAttributes qualification, [NotNull] Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();
            //check if mandatory fields presents

            if (qualification.QualificationCode.IsNullOrEmpty())
            {
                exceptionBuilder.AddException(ValidationExceptionType.MissingQualificationCode);
            }

            //check if StartDate < endDate if startDate and EndDate not null
            if ((qualification.StartDate != null && qualification.EndDate != null) && (qualification.StartDate.Value.Date > qualification.EndDate.Value.Date))
                exceptionBuilder.AddException(ValidationExceptionType.DateMismatch);

            //check if start date can not be less than apprentice DOB +12 years
            if (qualification.StartDate != null)
            {
                if (qualification.StartDate.Value.Date < profile.BirthDate.AddYears(+12))
                    exceptionBuilder.AddException(ValidationExceptionType.DOBDateMismatch);
            }
            //check if end date can not be less than apprentice DOB +12 years
            if (qualification.EndDate != null)
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

            exceptionBuilder.AddExceptions(await referenceDataValidator.ValidateAsync(qualification));

            return exceptionBuilder;
        }

        public ValidationExceptionBuilder CheckForDuplicates(List<PriorQualification> qualifications)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();
            //check for duplicates based on Qcode
            if (qualifications.GroupBy(x => x.QualificationCode.ToUpper()).Any(g => g.Count() > 1))
                exceptionBuilder.AddException(ValidationExceptionType.DuplicateQualification);
            return exceptionBuilder;
        }
    }
}