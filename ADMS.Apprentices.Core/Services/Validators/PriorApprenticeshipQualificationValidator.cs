using System;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Extensions;

// ReSharper disable PossibleInvalidOperationException

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class PriorApprenticeshipQualificationValidator : PriorQualificationValidator, IPriorApprenticeshipQualificationValidator
    {
        private readonly IReferenceDataValidator referenceDataValidator;

        public PriorApprenticeshipQualificationValidator(IReferenceDataValidator referenceDataValidator) : base(referenceDataValidator)
        {
            this.referenceDataValidator = referenceDataValidator;
        }

        public async Task<ValidationExceptionBuilder> ValidateAsync(PriorApprenticeshipQualification priorApprenticeship, Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();

            // start date cannot be less than apprentice DOB +12 years
            if (priorApprenticeship.StartDate.Value.Date < profile.BirthDate.AddYears(+12))
                exceptionBuilder.AddException(ValidationExceptionType.DOBDateMismatch);

            // start date cannot be greater than today's date.
            if (priorApprenticeship.StartDate.Value.Date > DateTime.Today)
                exceptionBuilder.AddException(ValidationExceptionType.InvalidDate);

            // at this time we only accept a single qualification manual reason code
            if (priorApprenticeship.QualificationManualReasonCode != null && priorApprenticeship.QualificationManualReasonCode != PriorApprenticeshipQualification.ManuallyEnteredCode)
                exceptionBuilder.AddException(ValidationExceptionType.InvalidQualificationManualReasonCode);

            // ANZSCO and level codes are required for a manually entered qualification code
            if (priorApprenticeship.QualificationManualReasonCode == PriorApprenticeshipQualification.ManuallyEnteredCode)
            {
                if (priorApprenticeship.QualificationANZSCOCode.IsNullOrWhitespace())
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidPriorApprenticeshipMissingAnzscoCode);
                if (priorApprenticeship.QualificationLevel.IsNullOrWhitespace())
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidPriorApprenticeshipMissingLevelCode);
            }

            exceptionBuilder.AddExceptions(await referenceDataValidator.ValidatePriorApprenticeshipQualificationsAsync(priorApprenticeship));
            return exceptionBuilder;
        }
    }
}