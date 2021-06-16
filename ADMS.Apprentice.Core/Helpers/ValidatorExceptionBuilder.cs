using System.Collections.Generic;
using System.Linq;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.Exceptions;


namespace ADMS.Apprentice.Core.Services.Validators
{
    public class ValidatorExceptionBuilder : List<ValidationExceptionType>, IValidatorExceptionBuilder
    {
        private readonly IExceptionFactory exceptionFactory;
        public ValidatorExceptionBuilder(IExceptionFactory exceptionFactory)
        {
            this.exceptionFactory = exceptionFactory;
        }

        public void AddExceptions(IValidatorExceptionBuilder exceptionBuilder)
        {
            this.AddRange(exceptionBuilder.GetExceptions());
        }

        public bool HasExceptions()
        {
            return this.Any();
        }
        
        public IEnumerable<ValidationExceptionType> GetExceptions()
        {
            return this;
        }

        public void ThrowAnyExceptions()
        {
            if(this.Any()){
                throw exceptionFactory.CreateValidationException(this.Distinct().ToArray());
            }
        }
    }
}