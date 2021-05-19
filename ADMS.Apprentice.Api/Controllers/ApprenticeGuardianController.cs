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
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IPagingHelper pagingHelper;
        private readonly IQualificationCreator qualificationCreator;
        private readonly IQualificationUpdater qualificationUpdater;
        private readonly IExceptionFactory exceptionFactory;

        /// <summary>Constructor</summary>
        public ApprenticeGuardianController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper,
            IQualificationCreator qualificationCreator,
            IQualificationUpdater qualificationUpdater,
            IExceptionFactory exceptionFactory
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.qualificationCreator = qualificationCreator;
            this.qualificationUpdater = qualificationUpdater;
            this.exceptionFactory = exceptionFactory;
        }

        /// <summary>
        /// Gets all information of a given qualification id.
        /// </summary>
        /// <param name="apprenticeId">Id of the apprentice</param>
        /// <param name="id">Id of the qualification</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileGuardianModel>> Get(int apprenticeId, int id)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            if (profile == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());
            Qualification qualification = profile.Qualifications.Single(x => x.Id == id);
            if (qualification == null)
                return NotFound();

            return Ok(new ProfileQualificationModel(qualification));
        }

        /// <summary>
        /// Adds a new qualification for an apprentice
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        /// <param name="message">Details of the qualification to be created</param>
        [HttpPost]
        public async Task<ActionResult<ProfileGuardianModel>> Create(int apprenticeId, [FromBody] ProfileGuardianMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            if (profile == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());
 
            return Created($"/{apprenticeId}", new ProfileGuardianModel());
        }
    }
}