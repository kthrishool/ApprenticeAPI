using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class PriorApprenticeshipValidator : QualificationValidator, IPriorApprenticeshipValidator
    {
        private readonly IReferenceDataValidator referenceDataValidator;

        public PriorApprenticeshipValidator(IReferenceDataValidator referenceDataValidator) : base(referenceDataValidator)
        {
            this.referenceDataValidator = referenceDataValidator;
        }


        public async Task<ValidationExceptionBuilder> ValidateAsync(PriorApprenticeship priorApprenticeship, Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();
            var tasks = new List<Task>();

            tasks.Add(base.ValidateAsync(priorApprenticeship, profile));
            tasks.Add(referenceDataValidator.PriorApprenticeshipValidator(priorApprenticeship));

            await tasks.WaitAndThrowAnyExceptionFound();


            return exceptionBuilder;
        }
    }
}