using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.HttpClients.USI;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using Adms.Shared;
using Adms.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentices.Api.Configuration;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// Apprentice usi endpoints.
    /// </summary>
    [Route("api/v1/apprentices/{apprenticeId}/usi")]
    [Route("api/apprentices/{apprenticeId}/usi")]
    [ApiController]
    //[ApiDescription(Summary = "Apprentice usi endpoints", Description = "")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [AllowAnonymous]
    //TODO: Is this meant to be anonymous? I'm not sure.
    public class ApprenticeUSIController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IUSIVerify usiVerify;

        /// <summary>Constructor</summary>
        public ApprenticeUSIController(
            IRepository repository,
            IUSIVerify usiVerify
        )
        {
            this.repository = repository;
            this.usiVerify = usiVerify;
        }

        /// <summary>
        /// Verify provided USI of a given apprentice
        /// </summary>
        /// <param name="apprenticeId"></param>
        /// <returns></returns>
        /// TODO: Confirm this is the appropriate policy.
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPost("verify")]
        public async Task<ActionResult<VerifyUsiModel>> Verify(int apprenticeId)
        {
            Profile profile = repository.Get<Profile>(apprenticeId);
            if (profile == null)
                throw AdmsNotFoundException.Create("Apprentice Profile", apprenticeId.ToString());
            ApprenticeUSI apprenticeUSI = usiVerify.Verify(profile);
            await repository.SaveAsync();
            if (apprenticeUSI == null)
                return Ok("No USI to be verified");
            return Ok(new ProfileUSIModel(apprenticeUSI));
        }
    }
}