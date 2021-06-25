using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.HttpClients.USI;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// Apprentice usi endpoints.
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
        /// <returns></returns>
        [HttpPost("verify")]
        public async Task<ActionResult<VerifyUsiModel>> Verify(int apprenticeId)
        {
            Profile profile = repository.Get<Profile>(apprenticeId);
            if (profile == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());
            ApprenticeUSI apprenticeUSI = usiVerify.Verify(profile);
            await repository.SaveAsync();
            if (apprenticeUSI == null)
                return Ok("No USI to be verified");
            return Ok(new ProfileUSIModel(apprenticeUSI));
        }
    }
}