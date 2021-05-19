using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Adms.Shared;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class GuardianValidator : IGuardianValidator
    {
        private readonly IRepository repository;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IReferenceDataClient referenceDataClient;
        private Guardian localGuardian;
        private readonly IPhoneValidator phoneValidator;

        public GuardianValidator(
            IRepository repository,
            IExceptionFactory exceptionFactory,
            IReferenceDataClient referenceDataClient,
            IPhoneValidator phoneValidator
        )
        {
            this.repository = repository;
            this.exceptionFactory = exceptionFactory;
            this.referenceDataClient = referenceDataClient;
            this.phoneValidator = phoneValidator;
        }

        public Guardian ValidateAsync(Guardian guardian)
        {
            localGuardian = guardian;
            PhoneValidation();
            return guardian; 
        }

        private void PhoneValidation()
        {
            localGuardian.LandLine = phoneValidator.ValidatePhone(localGuardian.LandLine);
            localGuardian.Mobile = phoneValidator.ValidatePhone(localGuardian.Mobile);
            localGuardian.WorkPhoneNumber = phoneValidator.ValidatePhone(localGuardian.WorkPhoneNumber);
        }
    }
}