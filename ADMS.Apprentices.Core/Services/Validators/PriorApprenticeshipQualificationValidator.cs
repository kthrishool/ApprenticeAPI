using System;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;

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

            exceptionBuilder.AddExceptions(await referenceDataValidator.ValidatePriorApprenticeshipQualificationsAsync(priorApprenticeship));
            return exceptionBuilder;
        }
    }
}