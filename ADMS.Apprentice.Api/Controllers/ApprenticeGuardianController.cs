using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Adms.Shared.Filters;

namespace ADMS.Apprentice.Api.Controllers
{
    /// <summary>
    /// Apprentice guardian endpoints of a given apprentice.
    /// </summary>
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices/{apprenticeId}/guardians")]
    [Route("api/apprentices/{apprenticeId}/guardians")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeGuardianController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IGuardianCreator guardianCreator;
        private readonly IGuardianUpdater guardianUpdater;

        /// <summary>Constructor</summary>
        public ApprenticeGuardianController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IExceptionFactory exceptionFactory,
            IGuardianCreator guardianCreator,
            IGuardianUpdater guardianUpdater
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.exceptionFactory = exceptionFactory;
            this.guardianCreator = guardianCreator;
            this.guardianUpdater = guardianUpdater;
        }

        /// <summary>
        /// List all guardians of an apprentice.
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        [HttpGet]        
        public async Task<ActionResult<ProfileGuardianModel[]>> List(int apprenticeId)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            if (profile == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());            

            ProfileGuardianModel[] models = profile.Guardians
                .OrderByDescending(x => x.Id)
                .Select(x => new ProfileGuardianModel(x))
                .ToArray();
            return Ok(models);
        }

        /// <summary>
        /// Gets all information of a given guardian id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the parent / guardian</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileGuardianModel>> Get(int apprenticeId, int id)
        {
            Guardian guardian;
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            guardian = profile.Guardians.SingleOrDefault(x => x.Id == id);

            if(guardian == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice guardian", id.ToString());

            return Ok(new ProfileGuardianModel(guardian));
        }

        /// <summary>
        /// Adds a new guardian for an apprentice
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        /// <param name="message">Details of the guardian to be created</param>
        [HttpPost]
        public async Task<ActionResult<ProfileGuardianModel>> Create(int apprenticeId, [FromBody] ProfileGuardianMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            if (profile == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());

            Guardian guardian = await guardianCreator.CreateAsync(message);
            profile.Guardians.Add(guardian);
            await repository.SaveAsync();
            return Created($"/{apprenticeId}", new ProfileGuardianModel(guardian));
        }

        /// <summary>
        /// Updates an existing parent/guardian.
        /// </summary>
        /// <param name="apprenticeId">ID of the apprentice</param>
        /// <param name="id">ID of the  parent/guardian to be updated</param>
        /// <param name="message">Details of the information to be updated</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProfileGuardianModel>> Update(int apprenticeId, int id, [FromBody] ProfileGuardianMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            if (profile == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());

            Guardian guardian = profile.Guardians.SingleOrDefault(x => x.Id == id);
            if (guardian == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice guardian ", id.ToString());

            await guardianUpdater.Update(guardian, message);

            await repository.SaveAsync();
            return Ok(new ProfileGuardianModel(guardian));
        }
    }
}