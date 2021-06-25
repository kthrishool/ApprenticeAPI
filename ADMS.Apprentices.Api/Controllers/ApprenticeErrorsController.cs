
using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.Core.Services;
using Adms.Shared.Exceptions;
using System;
using ADMS.Apprentices.Core.Exceptions;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    ///     List all the error messages available.
    /// </summary>
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices/errors")]
    [Route("api/apprentices/errors")]
    //[Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeErrorsController : AdmsController
    {
        private static IDictionary<string, (string, string)[]> errorsDictionary;
        private readonly IExceptionFactory exceptionFactory;
        
        static ApprenticeErrorsController(){
            errorsDictionary = new Dictionary<string, (string,string)[]>();
            errorsDictionary.Add("Validation Exceptions", GetValues<ValidationExceptionType>());
        }
        private static (string, string)[] GetValues<T>()
            where T: struct, Enum
        {
            var enumValues = Enum.GetValues<T>();
            return enumValues.Select(e => e.GetAttribute<ExceptionDetailsAttribute>())
                .Select(ed => (ed.ValidationRuleId, ed.Message)).ToArray();
        }

        /// <summary>Constructor</summary>
        public ApprenticeErrorsController(IHttpContextAccessor contextAccessor, IExceptionFactory exceptionFactory) : base(contextAccessor)
        {
            this.exceptionFactory = exceptionFactory;
        }

        /// <summary>List available error message </summary>
        [HttpGet]
        public string[] List()
        {
            return errorsDictionary.Keys.ToArray();
        }

        /// <summary>Get the details of errors for the specified errorType</summary>
        [HttpGet("{errorType}")]
        public (string, string)[] Get(string errorType)
        {            
            if (!errorsDictionary.ContainsKey(errorType)) {
                throw exceptionFactory.CreateNotFoundException("Error Type", errorType);
            }
            return errorsDictionary[errorType];
        }
    }
}