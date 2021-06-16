using Adms.Shared.Exceptions;


namespace ADMS.Apprentice.Core.Services.Validators
{
    public class ValidatorExceptionBuilderFactory : IValidatorExceptionBuilderFactory
    {
        private readonly IExceptionFactory exceptionFactory;
        
        public ValidatorExceptionBuilderFactory(IExceptionFactory exceptionFactory)
        {
            this.exceptionFactory = exceptionFactory;
        }
        public IValidatorExceptionBuilder CreateExceptionBuilder()
        {
            return new ValidatorExceptionBuilder(exceptionFactory);
        }
    }
}