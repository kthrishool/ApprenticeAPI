using System.Collections.Generic;
using ADMS.Apprentice.Core.Exceptions;


namespace ADMS.Apprentice.Core.Services.Validators
{
    /// <summary>
    /// The IExceptionBuilder adds exceptions and will throw exceptions if it has any.
    /// </summary>
    public interface IValidatorExceptionBuilder
    {
        /// <summary>
        /// Add an ValidationExceptionType to the builder.
        /// </summary>
        void Add(ValidationExceptionType exceptionType);
        
        /// <summary>
        /// Add zero or many exceptions to the builder.
        /// </summary>
        void AddExceptions(IValidatorExceptionBuilder exceptionBuilder);
        
        /// <summary>
        /// If any exceptions exist in the builder throw an exception.
        /// </summary>
        void ThrowAnyExceptions();
        
        /// <summary>
        /// If any exceptions exist return an enumeration of them.
        /// </summary>
        IEnumerable<ValidationExceptionType> GetExceptions();
        
        /// <summary>
        /// If any exceptions exist return true.
        /// </summary>
        bool HasExceptions();
    }
}