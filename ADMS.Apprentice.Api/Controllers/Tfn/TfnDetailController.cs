using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Helpers;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ADMS.Apprentice.Api.Filters;
using Microsoft.EntityFrameworkCore;

namespace ADMS.Apprentice.Api.Controllers.Tfn
{
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/apprentices/{apprenticeId}/TFN")]
    [Route("api/apprentices/{apprenticeId}/TFN")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TfnDetailController : AdmsController
    {
        private readonly IRepository repository;
        private readonly ITfnDetailCreator tfnDetailCreator;
        private readonly ITfnDetailRetreiver tfnDetailRetreiver;

        public TfnDetailController(
            IHttpContextAccessor contextAccessor, 
            IRepository repository,
            ITfnDetailCreator tfnDetailCreator,
            ITfnDetailRetreiver tfnDetailRetreiver
            ) : base(contextAccessor)
        {
            this.repository = repository;
            this.tfnDetailCreator = tfnDetailCreator;
            this.tfnDetailRetreiver = tfnDetailRetreiver;
        }

        /// <summary>
        /// Get all information regarding the current active TFN record.
        /// </summary>
        /// <param name="apprenticeId">Id of the Apprentice.</param>
        [HttpGet]
        public async Task<ActionResult<TFNV1>> Get(int apprenticeId)
        {
            TfnDetail tfnDetail = await repository
                .Retrieve<TfnDetail>().FirstOrDefaultAsync(x => x.ApprenticeId == apprenticeId);

            return Ok(new TFNV1
            {
                ApprenticeId = tfnDetail.ApprenticeId,
                TaxFileNumber = tfnDetail.TFN
            });
        }


        /// <summary>
        /// Create a new tfnDetail record
        /// </summary>
        /// <remarks>
        /// Create a new tfn and return all details.
        /// </remarks>
        /// <param name="apprenticeId"></param>
        /// <param name="message">Details of the tfn to be created</param>
        /// <response code="201">Returns newly created tfn</response>
        [HttpPost("post.{format}"), FormatFilter]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<TFNV1>> Create(int apprenticeId, [FromBody] TFNV1 message)
        {
            message.ApprenticeId = apprenticeId;
            var model = await tfnDetailCreator.CreateTfnDetailAsync(message);
            await repository.SaveAsync();

            return Created($"/{model.Id}", model);
        }
    }
}