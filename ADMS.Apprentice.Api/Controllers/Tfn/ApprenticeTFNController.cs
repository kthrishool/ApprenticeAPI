using ADMS.Apprentice.Core.Messages.TFN;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Api.Controllers.Tfn
{
    /// <summary>
    /// Apprentice TFN endpoints.
    /// </summary>
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices/{apprenticeId}/TFN")]
    [Route("api/apprentices/{apprenticeId}/TFN")]
    [Public]
    [Produces("application/json","text/xml")]
    [Consumes("application/json","text/xml")]
    public class ApprenticeTFNController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IApprenticeTFNCreator apprenticeTFNCreator;
        private readonly IApprenticeTFNRetreiver tfnDetailRetreiver;
        private readonly IApprenticeTFNUpdater apprenticeTFNUpdater;

        /// <summary>Constructor</summary>
        public ApprenticeTFNController(
            IHttpContextAccessor contextAccessor, 
            IRepository repository,
            IApprenticeTFNCreator apprenticeTFNCreator,
            IApprenticeTFNRetreiver tfnDetailRetreiver,
            IApprenticeTFNUpdater apprenticeTFNUpdater
            ) : base(contextAccessor)
        {
            this.repository = repository;
            this.apprenticeTFNCreator = apprenticeTFNCreator;
            this.tfnDetailRetreiver = tfnDetailRetreiver;
            this.apprenticeTFNUpdater = apprenticeTFNUpdater;
        }

        /// <summary>
        /// Get all information regarding the current active TFN record.
        /// </summary>
        /// <param name="apprenticeId">Id of the Apprentice.</param>
        [HttpGet]
        [Produces("application/json", "application/xml")]
        public ActionResult<ApprenticeTFNV1> Get(int apprenticeId)
        {
            var m =  tfnDetailRetreiver.Get(apprenticeId);

            return  Ok(m);
        }


        /// <summary>
        /// Create a new ApprenticeTFN record
        /// </summary>
        /// <remarks>
        /// Create a new ApprenticeTFN record and return details.
        /// </remarks>
        /// <param name="apprenticeId"></param>
        /// <param name="message">Details of the tfn to be created</param>
        /// <response code="201">Returns newly created tfn</response>
        [HttpPost]
        [Consumes("application/json", "application/xml", "text/xml")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ApprenticeTFNV1>> Post(int apprenticeId, [FromBody] ApprenticeTFNV1 message)
        {
            message.ApprenticeId = apprenticeId;
            var model = await apprenticeTFNCreator.CreateAsync(message);

            return Created($"/{message.ApprenticeId}", message);
        }

        /// <summary>
        /// Patch an ApprenticeTFN record
        /// </summary>
        /// <remarks>
        /// patch an existing ApprenticeTFN record and return all details.
        /// </remarks>
        /// <param name="apprenticeId"></param>
        /// <param name="message">Details of the tfn to be patched</param>
        /// <response code="201">Returns newly created tfn</response>
        [HttpPatch]
        [Consumes("application/json", "application/xml", "text/xml")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ApprenticeTFNV1>> Patch(int apprenticeId, [FromBody] ApprenticeTFNV1 message)
        {
            message.ApprenticeId = apprenticeId;
            await apprenticeTFNUpdater.Update(message);

            return Created($"/{message.ApprenticeId}", message);
        }
    }
}