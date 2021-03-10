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

namespace ADMS.Apprentice.Api.Controllers.Tfn
{
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/Apprentices/{ApprenticeId}/TFN")]
    [Route("api/Apprentices/{ApprenticeId}/TFN")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TfnDetailController : AdmsController
    {
        private readonly IRepository repository;
        private readonly ITfnDetailCreator tfnDetailCreator;

        public TfnDetailController(
            IHttpContextAccessor contextAccessor, 
            IRepository repository, 
            ITfnDetailCreator tfnDetailCreator
            ) : base(contextAccessor)
        {
            this.repository = repository;
            this.tfnDetailCreator = tfnDetailCreator;
        }

        /// <summary>
        /// Get all generic information regarding a single claim application regardless of its claim form category.
        /// To get all information regarding a single claim application, use the claim form category specific endpoint.
        /// </summary>
        /// <param name="id">Id of the claim application</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<TfnDetailModel>> Get(int id)
        {
            TfnDetail tfnDetail = await repository
                .Retrieve<TfnDetail>()
                .GetAsync(id);
            return Ok(new TfnDetailModel(tfnDetail));
        }


        /// <summary>
        /// Create a new tfnDetail record
        /// </summary>
        /// <remarks>
        /// Create a new tfn and return all details.
        /// </remarks>
        /// <param name="message">Details of the tfn to be created</param>
        /// <response code="201">Returns newly created tfn</response>
        [HttpPost("post.{format}"), FormatFilter]
        [DbWrite]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<TfnDetailModel>> Create(TfnCreateMessage message)
        {
            var model = await tfnDetailCreator.CreateTfnDetailAsync(message);

            return Created($"/{model.Id}", model);
        }
    }
}