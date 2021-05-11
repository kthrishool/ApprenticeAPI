using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.HttpClients.USI;

namespace ADMS.Apprentice.Api.Controllers
{
    /// <summary>
    /// Apprentice endpoints.
    /// </summary>
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices/{apprenticeId}/usi")]
    [Route("api/apprentices/{apprenticeId}/usi")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeUSIController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IUSIVerify usiVerify;
        private readonly IExceptionFactory exceptionFactory;

        /// <summary>Constructor</summary>
        public ApprenticeUSIController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,   
            IExceptionFactory exceptionFactory,
            IUSIVerify usiVerify
        ) : base(contextAccessor)
        {
            this.repository = repository;           
            this.usiVerify = usiVerify;
            this.exceptionFactory = exceptionFactory;
        }

        /// <summary>
        /// Verify provided USI of a given apprentice
        /// </summary>     
        /// <param name="apprenticeId"></param>
        /// <param name="usi"></param>
        /// <returns></returns>
        [HttpPost("{usi}/verify")]
        public async Task<ActionResult<VerifyUsiModel>> Verify(int apprenticeId, string usi)
        {
            //Profile profile = repository.Get<Profile>(apprenticeId, false);
            //if (profile == null)
            //    throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());

            ////get the active apprenticeUsi record.
            //ApprenticeUSI apprenticeUSI = profile.USIs.FirstOrDefault(x => x.ActiveFlag == true && x.USI == usi);
            //if (apprenticeUSI == null)
            //    throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());
            
            await usiVerify.VerifyAsync(apprenticeId, usi);

            Profile profile = repository.Get<Profile>(apprenticeId, false);
            ApprenticeUSI apprenticeUSI = profile.USIs.FirstOrDefault(x => x.ActiveFlag == true && x.USI == usi);

            return Ok(new USIVerifyResultModel(apprenticeUSI));
        }
    }
}