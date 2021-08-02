using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using Adms.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentices.Api.Configuration;
using Adms.Shared.Filters;
namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// Apprentice guardian endpoints of a given apprentice.
    /// </summary>
    [Route("api/v1/apprentices/{apprenticeId}/guardian")]
    [Route("api/apprentices/{apprenticeId}/guardian")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeGuardianController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IGuardianCreator guardianCreator;
        private readonly IGuardianRetriever guardianRetriever;
        private readonly IGuardianUpdater guardianUpdater;

        /// <summary>Constructor</summary>
        public ApprenticeGuardianController(
            IRepository repository,
            IGuardianCreator guardianCreator,
            IGuardianRetriever guardianRetriever,
            IGuardianUpdater guardianUpdater
        ) 
        {
            this.repository = repository;
            this.guardianCreator = guardianCreator;
            this.guardianRetriever = guardianRetriever;
            this.guardianUpdater = guardianUpdater;
        }

        /// <summary>
        /// Gets guardian information of a given apprentice id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [HttpGet]
        public async Task<ActionResult<ProfileGuardianModel>> Get(int apprenticeId)
        {
            Guardian guardian = await guardianRetriever.GetAsync(apprenticeId);
            return Ok(new ProfileGuardianModel(guardian));
        }

        /// <summary>
        /// Adds a guardian for an apprentice
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="message">Details of the guardian to be created</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPost]
        public async Task<ActionResult<ProfileGuardianModel>> Create(int apprenticeId, [FromBody] ProfileGuardianMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            Guardian guardian = await guardianCreator.CreateAsync(apprenticeId, message);
            profile.Guardian = guardian;
            await repository.SaveAsync();
            return Created($"/{guardian.Id}", new ProfileGuardianModel(guardian));
        }

        /// <summary>
        /// Updates an existing parent/guardian.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="message">Details of the information to be updated</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPut]
        public async Task<ActionResult<ProfileGuardianModel>> Update(int apprenticeId, [FromBody] ProfileGuardianMessage message)
        {
            Guardian guardian = await guardianRetriever.GetAsync(apprenticeId);
            await guardianUpdater.Update(guardian, message);
            await repository.SaveAsync();
            return Ok(new ProfileGuardianModel(guardian));
        }
    }
}