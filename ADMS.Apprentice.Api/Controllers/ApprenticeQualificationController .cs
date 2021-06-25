using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADMS.Apprentice.Api.Controllers
{
    /// <summary>
    /// Apprentice qualification endpoints of a given apprentice.
    /// </summary>
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices/{apprenticeId}/qualifications")]
    [Route("api/apprentices/{apprenticeId}/qualifications")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeQualificationController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IQualificationCreator qualificationCreator;
        private readonly IQualificationUpdater qualificationUpdater;
        private readonly ICollectionHelper collectionHelper;

        /// <summary>Constructor</summary>
        public ApprenticeQualificationController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IQualificationCreator qualificationCreator,
            IQualificationUpdater qualificationUpdater,
            ICollectionHelper collectionHelper
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.qualificationCreator = qualificationCreator;
            this.qualificationUpdater = qualificationUpdater;
            this.collectionHelper = collectionHelper;
        }

        /// <summary>
        /// List all qualifications for an apprentice.
        /// </summary>
        /// <param name="apprenticeId">ID of the apprentice</param>
        [HttpGet]
        public async Task<ActionResult<ProfileQualificationModel[]>> List(int apprenticeId)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            ProfileQualificationModel[] models = profile.Qualifications.Map(a => new ProfileQualificationModel(a)).ToArray();
            return Ok(models);
        }

        /// <summary>
        /// Gets all information of a given qualification id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the qualification</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileQualificationModel>> Get(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            Qualification qualification = collectionHelper.Get(profile.Qualifications, q => q.Id, id);
            return Ok(new ProfileQualificationModel(qualification));
        }

        /// <summary>
        /// Adds a new qualification for an apprentice
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        /// <param name="message">Details of the qualification to be created</param>
        [HttpPost]
        public async Task<ActionResult<ProfileQualificationModel>> Create(int apprenticeId, [FromBody] ProfileQualificationMessage message)
        {
            Qualification qualification = await qualificationCreator.CreateAsync(apprenticeId, message);
            await repository.SaveAsync();
            return Created($"/{qualification.Id}", new ProfileQualificationModel(qualification));
        }

        /// <summary>
        /// Updates an existing qualification claim application.
        /// </summary>
        /// <param name="apprenticeId">ID of the apprentice</param>
        /// <param name="id">ID of the qualification to be updated</param>
        /// <param name="message">Details of the information to be updated</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProfileQualificationModel>> Update(int apprenticeId, int id, [FromBody] ProfileQualificationMessage message)
        {
            var qualification = await qualificationUpdater.Update(apprenticeId, id, message);
            await repository.SaveAsync();
            return Ok(new ProfileQualificationModel(qualification));
        }

        /// <summary>
        /// Removes a qualification from an apprentice profile
        /// </summary>
        /// <param name="apprenticeId">ID of the apprentice</param>
        /// <param name="id">ID of the qualification to be removed</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId);
            Qualification qualification = collectionHelper.Get(profile.Qualifications, q => q.Id, id);
            profile.Qualifications.Remove(qualification);
            await repository.SaveAsync();
            return new NoContentResult();
        }
    }
}