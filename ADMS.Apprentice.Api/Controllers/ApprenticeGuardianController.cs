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
        private readonly IExceptionFactory exceptionFactory;
        private readonly IGuardianCreator guardianCreator;

        /// <summary>Constructor</summary>
        public ApprenticeGuardianController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper,
            IExceptionFactory exceptionFactory,
            IGuardianCreator guardianCreator
        ) : base(contextAccessor)
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.exceptionFactory = exceptionFactory;
            this.guardianCreator = guardianCreator;
        }


        /// <summary>
        /// Adds a new guardian for an apprentice
        /// </summary>
        /// <param name="apprenticeId">apprenticeId</param>
        /// <param name="message">Details of the qualification to be created</param>
        [HttpPost]
        public async Task<ActionResult<ProfileGuardianModel>> Create(int apprenticeId, [FromBody] ProfileGuardianMessage message)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            if (profile == null)
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", apprenticeId.ToString());

            Guardian guardian = await guardianCreator.CreateAsync(message);
            profile.Guardians.Add(guardian);
            await repository.SaveAsync();
            return Created($"/{apprenticeId}", new ProfileGuardianModel());
        }
    }
}