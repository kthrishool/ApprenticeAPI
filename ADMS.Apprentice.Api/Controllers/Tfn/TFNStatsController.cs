using System;
using ADMS.Apprentice.Core.Messages.TFN;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Adms.Shared.Extensions;

namespace ADMS.Apprentice.Api.Controllers.Tfn
{
    /// <summary>TFNStats endpoints</summary>
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/tfnstats")]
    [Route("api/tfnstats")]
    [Public]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TFNStatsController : AdmsController
    {
        private readonly IPagingHelper _pagingHelper;
        private readonly ITFNStatsRetriever _tfnStatsRetriever;

        /// <summary>Constructor</summary>
        public TFNStatsController(
            IHttpContextAccessor contextAccessor,
            IPagingHelper pagingHelper,
            ITFNStatsRetriever tfnStatsRetriever) : base(contextAccessor)
        {
            _pagingHelper = pagingHelper;
            _tfnStatsRetriever = tfnStatsRetriever;
        }

        /// <summary>
        /// Get all stats related to TFN records.
        /// </summary>
        /// <param name="paging">paging metadata.</param>
        /// <param name="criteria">Search criteria</param>
        [HttpGet]
        [SupportsPaging(typeof(TFNStatsCriteria))]
        public ActionResult<PagedList<TFNStatsV1>> List(PagingInfo paging, TFNStatsCriteria criteria)
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("CreatedOn", true);

            var tfnRecords = _tfnStatsRetriever.RetrieveTfnStats(criteria);

            
            var systemDateTime = Context == null || Context.DateTimeContext == DateTime.MinValue
                ? DateTime.Now
                : Context.DateTimeContext;

            PagedList<ApprenticeTFN> tfnPagedList = _pagingHelper.ToPagedList(tfnRecords, paging);

            
            return Ok(new PagedList<TFNStatsV1>(tfnPagedList, tfnPagedList.Results.Map(x => new TFNStatsV1(
               x.ApprenticeId,
               x.Profile.FirstName + " " + x.Profile.Surname,
               x.Profile.BirthDate,
               x.StatusDate,
               x.CreatedOn,
               x.StatusCode.ToString(),
               x.StatusCode == TFNStatus.NOCH ? GetNumberOfDaysSinceTheMismatchAsString(x.StatusDate, systemDateTime) : "-"
               ))));
        }

        
        private static int GetNumberOfDaysSinceTheMismatch(DateTime tfnStatusDate, DateTime systemDateTime)
        {
            return systemDateTime.Subtract(tfnStatusDate).Days > 0 ? systemDateTime.Subtract(tfnStatusDate).Days : 0;
        }

        private static string GetNumberOfDaysSinceTheMismatchAsString(DateTime tfnStatusDate, DateTime systemDateTime)
        {
            var number = GetNumberOfDaysSinceTheMismatch(tfnStatusDate, systemDateTime);
            return number > 0 ? number.ToString() : "<1";
        }


        


    }
}