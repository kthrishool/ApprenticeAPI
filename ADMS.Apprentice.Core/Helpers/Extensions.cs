using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Helpers
{
    public static class StringExtensions
    {
        //TODO: need to move to AMDS shared
        public static string Sanitise(this String str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) ? null : str.Trim();
        }

        public static string SanitiseUpper(this String str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) ? null : str.Trim().ToUpper();
        }
    }
}


namespace ADMS.Apprentice.Core.Services.Validators
{
    public static class ValidatorExtensions
    {
        /// <summary>
        /// For all the tasks from validators execute them and if an exception occurs throw the first one found.
        /// </summary>
        /// <param name="tasksWaiting">For the list of tasks</param>
        /// <example>
        ///     <code>
        ///         await tasks.WaitAndThrowAnyExceptionFound();
        ///     </code>
        /// </example>
        public async static Task WaitAndThrowAnyExceptionFound(this IEnumerable<Task> tasksWaiting)
        { 
            var tasks = new List<Task>();
            tasks.AddRange(tasksWaiting);
            while(tasks.Any())
            {
                Task completedTask = await Task.WhenAny(tasks);
                if(completedTask.Exception != null)
                {
                    throw completedTask.Exception.InnerException;
                }
                tasks.Remove(completedTask);
            }
        }

        /// <summary>
        /// For all the tasks which return IValditatorExceptionBuilder from validators execute them and aggregate the exceptions into one IValidationExceptionBuilder.
        /// If an exception thrown from one of the tasks throw it as soon as it occurs.
        /// </summary>
        /// <param name="tasksWaiting">For the list of tasks</param>
        /// <returns>IValidationExceptionBuilder with exceptions from all of the tasks</returns>
        /// <example>
        ///     <code>
        ///         await tasks.WaitAndThrowAnyExceptionFound();
        ///     </code>
        /// </example>
        public async static Task<ValidationExceptionBuilder> WaitAndReturnExceptionBuilder(this IEnumerable<Task<ValidationExceptionBuilder>> tasksWaiting)
        { 
            var tasks = new List<Task<ValidationExceptionBuilder>>();
            tasks.AddRange(tasksWaiting);
            ValidationExceptionBuilder exceptionBuilder = null;
            while(tasks.Any())
            {
                Task<ValidationExceptionBuilder> completedTask = await Task.WhenAny(tasks);
                if(completedTask.Exception != null)
                {
                    throw completedTask.Exception;
                }
                if(exceptionBuilder == null) {
                    exceptionBuilder = completedTask.Result;
                } else {
                    exceptionBuilder.AddExceptions(completedTask.Result);
                }
                tasks.Remove(completedTask);
            }
            return exceptionBuilder;
        }
    }
}