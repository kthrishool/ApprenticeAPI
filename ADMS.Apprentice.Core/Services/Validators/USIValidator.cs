using System;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Helpers;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services.Validators
{
    public class USIValidator : IUSIValidator
    {
        private readonly IExceptionFactory exceptionFactory;
        private readonly IReferenceDataValidator referenceDataValidator;


        public USIValidator(IExceptionFactory exceptionFactory, IReferenceDataValidator referenceDataValidator)
        {
            this.exceptionFactory = exceptionFactory;
            this.referenceDataValidator = referenceDataValidator;
        }


        public Boolean Validate(Profile profile)
        {
            if (profile.USIs.Any())
            {
                if (profile.USIs.Single().USI.Sanitise() == null)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidUSI);

                // code to be implemented fro additional validation
            }
            return true;
        }
    }
}