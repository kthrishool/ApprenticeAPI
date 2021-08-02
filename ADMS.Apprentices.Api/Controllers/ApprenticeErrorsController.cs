using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Api.Configuration;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// List all the error messages available.
    /// </summary>
    [Route("api/v1/apprentices/errors")]
    [Route("api/apprentices/errors")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeErrorsController : ControllerBase
    {
        private static readonly IDictionary<string, (string, string)[]> errorsDictionary;
        
        static ApprenticeErrorsController(){
            errorsDictionary = new Dictionary<string, (string, string)[]>
            {
                { "Validation Exceptions", GetValues<ValidationExceptionType>() }
            };
        }

        private static (string, string)[] GetValues<T>()
            where T : struct, Enum
        {
            var enumValues = Enum.GetValues<T>();
            return enumValues.Select(e => e.GetAttribute<ExceptionDetailsAttribute>())
                .Select(ed => (ed.ValidationRuleId, ed.Message)).ToArray();
        }

        /// <summary>List available error messages.</summary>
        [HttpGet]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_ITAdmin)]
        public string[] List()
        {
            return errorsDictionary.Keys.ToArray();
        }

        /// <summary>Get the details of errors for the specified errorType.</summary>
        [HttpGet("{errorType}")]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_ITAdmin)]
        public (string, string)[] Get(string errorType)
        {            
            if (!errorsDictionary.ContainsKey(errorType)) {
                throw AdmsNotFoundException.Create("Error Type", errorType);
            }
            return errorsDictionary[errorType];
        }
    }
}