using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentice.Core.Messages.TFN;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using ADMS.Apprentice.Core.Models.TFN;
using Adms.Shared.Extensions;
using Castle.Components.DictionaryAdapter;
using Microsoft.EntityFrameworkCore;

namespace ADMS.Apprentice.Api.Controllers.Tfn
{
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/tfnstats")]
    [Route("api/tfnstats")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TFNStatsController : AdmsController
    {
        private readonly IPagingHelper _pagingHelper;

        private readonly IRepository repository;
        private readonly IApprenticeTFNCreator apprenticeTFNCreator;
        private readonly IApprenticeTFNRetreiver tfnDetailRetreiver;
        private readonly IApprenticeTFNUpdater apprenticeTFNUpdater;

        public TFNStatsController(
            IHttpContextAccessor contextAccessor, 
            IRepository repository,
            IApprenticeTFNCreator apprenticeTFNCreator,
            IApprenticeTFNRetreiver tfnDetailRetreiver,
            IApprenticeTFNUpdater apprenticeTFNUpdater, 
            IPagingHelper pagingHelper) : base(contextAccessor)
        {
            this.repository = repository;
            this.apprenticeTFNCreator = apprenticeTFNCreator;
            this.tfnDetailRetreiver = tfnDetailRetreiver;
            this.apprenticeTFNUpdater = apprenticeTFNUpdater;
            _pagingHelper = pagingHelper;
        }

        /// <summary>
        /// Get all stats related to TFN records.
        /// </summary>
        /// <param name="paging">paging metadata.</param>
        [HttpGet]
        [SupportsPaging(null)]
        public async Task<ActionResult<PagedList<TFNStatsV1>>> List(PagingInfo paging)
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("ApprenticeId", true);

            var tfnRecords = repository.Retrieve<ApprenticeTFN>().Include(x => x.Profile);
            

            PagedList<ApprenticeTFN> tfnPagedList = await _pagingHelper.ToPagedListAsync(tfnRecords, paging);

            var model = new List<TFNStatsModel>();

            model.AddRange(tfnRecords.Select(x => (TFNStatsModel)x));


            return  Ok(new PagedList<TFNStatsV1>(tfnPagedList, tfnPagedList.Results.Map(x => new TFNStatsV1(
                x.ApprenticeId, 
                x.Profile?.FirstName + " " + x.Profile?.Surname,
                x.Profile?.BirthDate,
                x.StatusDate,
                x.CreatedOn,
                x.StatusCode.ToString(),
                ""
                ))));
        }

    }
}