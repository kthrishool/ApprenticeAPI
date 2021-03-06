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
        public PriorQualificationValidator()
        { }

        public ValidationExceptionBuilder ValidatePriorQualification(PriorQualification qualification, [NotNull] Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();

            //start date cannot be less than apprentice DOB +12 years
            if (qualification.StartDate != null)
            {
                if (qualification.StartDate.Value.Date < profile.BirthDate.AddYears(+12))
                    exceptionBuilder.AddException(ValidationExceptionType.DOBDateMismatch);
            }

            //end date can not be less than apprentice DOB +12 years
            if (qualification.EndDate != null)
            {
                if (qualification.EndDate.Value.Date < profile.BirthDate.AddYears(+12))
                    exceptionBuilder.AddException(ValidationExceptionType.DOBDateMismatch);
            }

            // start date and end date can not be greater than today's date.
            if (qualification.StartDate != null && qualification.EndDate != null)
            {
                if (qualification.StartDate.Value.Date >= qualification.EndDate.Value.Date)
                    exceptionBuilder.AddException(ValidationExceptionType.DateMismatch);
                if (qualification.StartDate.Value.Date > DateTime.Today || qualification.EndDate.Value.Date > DateTime.Today)
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidDate);
            }

            // at this time we only accept a single qualification manual reason code
            if (qualification.QualificationManualReasonCode != null && qualification.QualificationManualReasonCode != PriorQualification.ManuallyEnteredCode)
            {
                exceptionBuilder.AddException(ValidationExceptionType.InvalidQualificationManualReasonCode);
            }

            // ANZSCO and level codes are required for a manually entered qualification code
            if (qualification.QualificationManualReasonCode == PriorQualification.ManuallyEnteredCode)
            {
                if (qualification.QualificationANZSCOCode.IsNullOrWhitespace())
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidPriorQualificationMissingAnzscoCode);
                if (qualification.QualificationLevel.IsNullOrWhitespace())
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidPriorQualificationMissingLevelCode);
            }

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