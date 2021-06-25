using Adms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADMS.Apprentices.Api.Filters
{
    /// <summary>DbWriteAttribute</summary>
    public class DbWriteAttribute : TypeFilterAttribute
    {
        /// <summary>Constructor</summary>
        public DbWriteAttribute() : base(typeof(DbWriteFilter))
        {
        }

        private class DbWriteFilter : IActionFilter
        {
            private readonly IRepository repository;

            public DbWriteFilter(IRepository repository)
            {
                this.repository = repository;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                if (context.Exception == null)
                {
                    repository.Save();
                }
            }
        }
    }
}
