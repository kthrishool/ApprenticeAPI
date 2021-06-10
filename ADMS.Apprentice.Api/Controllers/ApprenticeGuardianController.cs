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
    [Route("api/v1/apprentices/{apprenticeId}/guardian")]
    [Route("api/apprentices/{apprenticeId}/guardian")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApprenticeGuardianController : AdmsController
    {
        private readonly IRepository repository;        
        private readonly IGuardianCreator guardianCreator;
        private readonly IGuardianRetreiver guardianRetreiver;
        private readonly IGuardianUpdater guardianUpdater;

        /// <summary>Constructor</summary>
        public ApprenticeGuardianController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,            
            IGuardianCreator guardianCreator,
            IGuardianRetreiver guardianRetreiver,
            IGuardianUpdater guardianUpdater
        ) : base(contextAccessor)
        {
            this.repository = repository;            
            this.guardianCreator = guardianCreator;
            this.guardianRetreiver = guardianRetreiver;
            this.guardianUpdater = guardianUpdater;
        }

        /// <summary>
        /// Gets guardian information of a given apprentice id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>        
        [HttpGet]
        public async Task<ActionResult<ProfileGuardianModel>> Get(int apprenticeId)
        {
            Guardian guardian = await guardianRetreiver.GetAsync(apprenticeId);
            return Ok(new ProfileGuardianModel(guardian));
        }

        /// <summary>
        /// Adds a guardian for an apprentice
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        /// <param name="message">Details of the guardian to be created</param>
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
        /// <param name="apprenticeId">ID of the apprentice</param>        
        /// <param name="message">Details of the information to be updated</param>
        [HttpPut]
        public async Task<ActionResult<ProfileGuardianModel>> Update(int apprenticeId, [FromBody] ProfileGuardianMessage message)
        {
            Guardian guardian = await guardianRetreiver.GetAsync(apprenticeId);
            await guardianUpdater.Update(guardian, message);
            await repository.SaveAsync();
            return Ok(new ProfileGuardianModel(guardian));
        }
    }
}