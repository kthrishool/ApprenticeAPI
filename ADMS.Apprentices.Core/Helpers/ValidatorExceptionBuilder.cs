using System;
using System.Collections.Generic;
using System.Linq;
using Adms.Shared.Exceptions;
using ADMS.Apprentices.Core.Exceptions;


namespace ADMS.Apprentices.Core.Services.Validators
{
    public class ValidationExceptionBuilder : ExceptionBuilder<ValidationExceptionType>
    {
        public ValidationExceptionBuilder(IExceptionFactory exceptionFactory) : base(exceptionFactory)
        {

        }

    }
    public class ExceptionBuilder<TExceptionType>
        where TExceptionType : Enum
    {
        private readonly IExceptionFactory exceptionFactory;
        private readonly List<TExceptionType> exceptions;
        public ExceptionBuilder(IExceptionFactory exceptionFactory)
        {
            this.exceptionFactory = exceptionFactory;
            this.exceptions = new List<TExceptionType>();
        }
        
        public void AddException(TExceptionType exceptionType) {
            exceptions.Add(exceptionType);
        }

        public void AddExceptions(IEnumerable<TExceptionType> exceptionBuilder)
        {
            exceptions.AddRange(exceptionBuilder);
        }

        public void AddExceptions(ExceptionBuilder<TExceptionType> exceptionBuilder)
        {
            exceptions.AddRange(exceptionBuilder.GetValidationExceptions());
        }

        public bool HasExceptions()
        {
            return exceptions.Any();
        }
        
        public IEnumerable<TExceptionType> GetValidationExceptions()
        {
            return exceptions;
        }

        public void ThrowAnyExceptions()
        {
            if(exceptions.Any()){
                throw exceptionFactory.CreateValidationException<TExceptionType>(exceptions.Distinct().ToArray());
            }
        }
    }
}