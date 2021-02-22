using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.ClaimSubmissions.Entities;
using ADMS.Apprentice.Core.ClaimSubmissions.Messages;
using ADMS.Apprentice.Core.ClaimSubmissions.Models;
using ADMS.Apprentice.Core.ClaimSubmissions.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADMS.Apprentice.Api.Controllers
{
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/claim-submissions")]
    [Route("api/claim-submissions")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ClaimSubmissionsController : AdmsController
    {
        private readonly IRepository repository;
        private readonly IPagingHelper pagingHelper;
        private readonly IClaimSubmissionCreator claimSubmissionCreator;

        public ClaimSubmissionsController(
            IHttpContextAccessor contextAccessor, 
            IRepository repository, 
            IPagingHelper pagingHelper,
            IClaimSubmissionCreator claimSubmissionCreator
            ) : base(contextAccessor)
        {
            this.repository = repository;
            this.pagingHelper = pagingHelper;
            this.claimSubmissionCreator = claimSubmissionCreator;
        }

        [HttpGet]
        [SupportsPaging(null)]
        public async Task<ActionResult<PagedList<ClaimSubmissionListModel>>> List(PagingInfo paging)
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("id", true);
            PagedList<ClaimSubmission> submissions = await pagingHelper.ToPagedListAsync(repository.Retrieve<ClaimSubmission>(), paging);
            IEnumerable<ClaimSubmissionListModel> models = submissions.Results.Map(a => new ClaimSubmissionListModel(a));
            return Ok(new PagedList<ClaimSubmissionListModel>(submissions, models));
        }

        [HttpPost]
        public async Task<ActionResult<ClaimSubmissionModel>> Create([FromBody]ClaimSubmissionMessage message)
        {
            ClaimSubmission submission = await claimSubmissionCreator.CreateAsync(message);
            await repository.SaveAsync();
            return Created($"/{submission.Id}", new ClaimSubmissionModel(submission));
        }
    }
}