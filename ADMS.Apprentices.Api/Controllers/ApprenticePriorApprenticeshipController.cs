using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Api.Configuration;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using Adms.Shared;
using Adms.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADMS.Apprentices.Api.Controllers
{
    /// <summary>
    /// Apprentice prior apprenticeship endpoints of a given apprentice.
    /// </summary>
    [ApiController]
    [Route("api/v1/apprentices/{apprenticeId}/prior-apprenticeships")]
    [Route("api/apprentices/{apprenticeId}/prior-apprenticeships")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticePriorApprenticeshipController : ControllerBase
    {
        private readonly IRepository repository;

        private readonly IPriorApprenticeshipCreator priorApprenticeshipCreator;
        private readonly IPriorApprenticeshipUpdater priorApprenticeshipUpdater;

        /// <summary>Constructor</summary>
        public ApprenticePriorApprenticeshipController(
            IRepository repository,
            IPriorApprenticeshipCreator priorApprenticeshipCreator,
            IPriorApprenticeshipUpdater priorApprenticeshipUpdater
        )
        {
            this.repository = repository;
            this.priorApprenticeshipCreator = priorApprenticeshipCreator;
            this.priorApprenticeshipUpdater = priorApprenticeshipUpdater;
        }

        /// <summary>
        /// List all prior apprenticeships for an apprentice.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// TODO DB design needs to be completed for this end point to work.
        /// EndPoint will be decorated with the role below when DB design is complete
        /// Linked to task 126782
        // [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_DENY_ALL)]
        [HttpGet]
        public async Task<ActionResult<ProfileQualificationModel[]>> List(int apprenticeId)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            ProfilePriorApprenticeshipModel[] models = profile.PriorApprenticeships.Map(a => new ProfilePriorApprenticeshipModel(a)).ToArray();
            return Ok(models);
        }

        /// <summary>
        /// Gets all information of a given prior apprenticeships id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the apprenticeship</param>
        /// TODO: DB design needs to be completed for this end point to work.
        /// EndPoint will be decorated with the role below when DB design is complete
        /// Linked to task 126782
        //[Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_DENY_ALL)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfilePriorApprenticeshipModel>> Get(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            PriorApprenticeship priorApprenticeship = profile.PriorApprenticeships.Get(q => q.Id, id);
            return Ok(new ProfilePriorApprenticeshipModel(priorApprenticeship));
        }

        /// <summary>
        /// Adds a new prior apprenticeship for an apprentice
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="message">Details of the apprenticeship to be created</param>
        /// TODO: DB design needs to be completed for this end point to work.
        /// EndPoint will be decorated with the role below when DB design is complete
        /// Linked to task 126782
        //[Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_DENY_ALL)]
        [HttpPost]
        public async Task<ActionResult<ProfilePriorApprenticeshipModel>> Create(int apprenticeId, [FromBody] ProfilePriorApprenticeshipMessage message)
        {
            PriorApprenticeship priorApprenticeship = await priorApprenticeshipCreator.CreateAsync(apprenticeId, message);
            await repository.SaveAsync();
            return Created($"/{priorApprenticeship.Id}", new ProfilePriorApprenticeshipModel(priorApprenticeship));
        }

        /// <summary>
        /// Updates an existing prior apprenticeship.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the prior apprenticeship to be updated</param>
        /// <param name="message">Details of the information to be updated</param>
        /// TODO: DB design needs to be completed for this end point to work.
        /// EndPoint will be decorated with the role below when DB design is complete
        /// Linked to task 126782
        // [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_DENY_ALL)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProfilePriorApprenticeshipModel>> Update(int apprenticeId, int id, [FromBody] ProfilePriorApprenticeshipMessage message)
        {
            // Need to throw an error if profile cannot be found as qualification validator doesn't support a profile with a null value.
            var profile = await repository.GetAsync<Profile>(apprenticeId, true);
            var priorApprenticeship = await priorApprenticeshipUpdater.Update(apprenticeId, id, message, profile);
            await repository.SaveAsync();
            return Ok(new ProfilePriorApprenticeshipModel(priorApprenticeship));
        }

        /// <summary>
        /// Removes a qualification from an apprentice profile
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the apprenticeship to be removed</param>
        /// TODO: DB design needs to be completed for this end point to work.
        /// EndPoint will be decorated with the role below when DB design is complete
        /// Linked to task 126782
        // [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_DENY_ALL)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId);
            PriorApprenticeship priorApprenticeship = profile.PriorApprenticeships.Get(q => q.Id, id);
            profile.PriorApprenticeships.Remove(priorApprenticeship);
            await repository.SaveAsync();
            return new NoContentResult();
        }
    }
}