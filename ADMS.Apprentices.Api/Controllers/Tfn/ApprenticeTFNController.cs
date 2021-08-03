using System.Threading.Tasks;
using ADMS.Apprentices.Core.Messages.TFN;
using ADMS.Apprentices.Core.Services;
using Adms.Shared;
using Adms.Shared.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentices.Api.Configuration;

namespace ADMS.Apprentices.Api.Controllers.Tfn
{
    /// <summary>
    /// Apprentice TFN endpoints.
    /// </summary>
    [Route("api/v1/apprentices/{apprenticeId}/TFN")]
    [Route("api/apprentices/{apprenticeId}/TFN")]
    [ApiController]
    //[ApiDescription(Summary = "Apprentice TFN endpoints", Description = "")]
    [Produces("application/json", "text/xml")]
    [Consumes("application/json", "text/xml")]
    public class ApprenticeTFNController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IApprenticeTFNCreator apprenticeTFNCreator;
        private readonly IApprenticeTFNRetriever tfnDetailRetriever;
        private readonly IApprenticeTFNUpdater apprenticeTFNUpdater;

        /// <summary>Constructor</summary>
        public ApprenticeTFNController(
            IRepository repository,
            IApprenticeTFNCreator apprenticeTFNCreator,
            IApprenticeTFNRetriever tfnDetailRetriever,
            IApprenticeTFNUpdater apprenticeTFNUpdater
        )
        {
            this.repository = repository;
            this.apprenticeTFNCreator = apprenticeTFNCreator;
            this.tfnDetailRetriever = tfnDetailRetriever;
            this.apprenticeTFNUpdater = apprenticeTFNUpdater;
        }

        /// <summary>
        /// Get all information regarding the current active TFN record.
        /// </summary>
        /// <param name="apprenticeId">Id of the Apprentice.</param>
        [HttpGet]
        [Produces("application/json", "application/xml")]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_TSL_Management)]
        public ActionResult<ApprenticeTFNV1> Get(int apprenticeId)
        {
            var m = tfnDetailRetriever.Get(apprenticeId);

            return Ok(m);
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
        [DuplicateRequestCheck("TFNCreate")]
        [Consumes("application/json", "application/xml", "text/xml")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_TSL_Management)]
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
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_TSL_Management)]
        public async Task<ActionResult<ApprenticeTFNV1>> Patch(int apprenticeId, [FromBody] ApprenticeTFNV1 message)
        {
            message.ApprenticeId = apprenticeId;
            await apprenticeTFNUpdater.Update(message);

            return Created($"/{message.ApprenticeId}", message);
        }
    }
}
