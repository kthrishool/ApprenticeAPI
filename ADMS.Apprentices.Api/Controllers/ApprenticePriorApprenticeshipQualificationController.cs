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
    [Route("v1/apprentices/{apprenticeId}/prior-apprenticeship-qualifications")]
    [Route("apprentices/{apprenticeId}/prior-apprenticeship-qualifications")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticePriorApprenticeshipQualificationController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IPriorApprenticeshipQualificationCreator priorApprenticeshipCreator;
        private readonly IPriorApprenticeshipQualificationUpdater priorApprenticeshipUpdater;

        /// <summary>Constructor</summary>
        public ApprenticePriorApprenticeshipQualificationController(
            IRepository repository,
            IPriorApprenticeshipQualificationCreator priorApprenticeshipCreator,
            IPriorApprenticeshipQualificationUpdater priorApprenticeshipUpdater
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
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [HttpGet]
        public async Task<ActionResult<PriorApprenticeshipQualificationModel[]>> List(int apprenticeId)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            PriorApprenticeshipQualificationModel[] models = profile.PriorApprenticeshipQualifications.Map(a => new PriorApprenticeshipQualificationModel(a)).ToArray();
            return Ok(models);
        }

        /// <summary>
        /// Gets all information of a given prior apprenticeships id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the prior apprenticeship</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_View)]
        [HttpGet("{id}")]
        public async Task<ActionResult<PriorApprenticeshipQualificationModel>> Get(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            PriorApprenticeshipQualification priorApprenticeship = profile.PriorApprenticeshipQualifications.Get(q => q.Id, id);
            return Ok(new PriorApprenticeshipQualificationModel(priorApprenticeship));
        }

        /// <summary>
        /// Adds a new prior apprenticeship for an apprentice
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="message">Details of the prior apprenticeship to be created</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPost]
        public async Task<ActionResult<PriorApprenticeshipQualificationModel>> Create(int apprenticeId, [FromBody] PriorApprenticeshipQualificationMessage message)
        {
            PriorApprenticeshipQualification priorApprenticeship = await priorApprenticeshipCreator.CreateAsync(apprenticeId, message);
            await repository.SaveAsync();
            return Created($"/{priorApprenticeship.Id}", new PriorApprenticeshipQualificationModel(priorApprenticeship));
        }

        /// <summary>
        /// Updates an existing prior apprenticeship.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the prior apprenticeship to be updated</param>
        /// <param name="message">Details of the information to be updated</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpPut("{id}")]
        public async Task<ActionResult<PriorApprenticeshipQualificationModel>> Update(int apprenticeId, int id, [FromBody] PriorApprenticeshipQualificationMessage message)
        {
            // Need to throw an error if profile cannot be found as prior apprenticeship validator doesn't support a profile with a null value.
            var profile = await repository.GetAsync<Profile>(apprenticeId, true);
            var priorApprenticeship = priorApprenticeshipUpdater.Update(apprenticeId, id, message, profile);
            await repository.SaveAsync();
            return Ok(new PriorApprenticeshipQualificationModel(priorApprenticeship));
        }

        /// <summary>
        /// Removes a qualification from an apprentice profile
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the prior apprenticeship to be removed</param>
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_Management)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId);
            PriorApprenticeshipQualification priorApprenticeship = profile.PriorApprenticeshipQualifications.Get(q => q.Id, id);
            profile.PriorApprenticeshipQualifications.Remove(priorApprenticeship);
            await repository.SaveAsync();
            return new NoContentResult();
        }
    }
}