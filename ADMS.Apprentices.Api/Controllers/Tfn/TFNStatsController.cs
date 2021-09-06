using System;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;
using ADMS.Apprentices.Core.Services;
using Adms.Shared.Extensions;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using Au.Gov.Infrastructure.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentices.Api.Configuration;

namespace ADMS.Apprentices.Api.Controllers.Tfn
{
    /// <summary>TFNStats endpoints</summary>
    [Route("v1/tfnstats")]
    [Route("tfnstats")]
    [ApiController]
    //[ApiDescription(Summary = "TFNStats endpoints", Description = "")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TFNStatsController : ControllerBase
    {
        private readonly IPagingHelper _pagingHelper;
        private readonly ITFNStatsRetriever _tfnStatsRetriever;
        private readonly ICallerContext _callerContext;

        /// <summary>Constructor</summary>
        public TFNStatsController(
            IPagingHelper pagingHelper,
            ITFNStatsRetriever tfnStatsRetriever,
            ICallerContext callerContext)
        {
            _pagingHelper = pagingHelper;
            _tfnStatsRetriever = tfnStatsRetriever;
            _callerContext = callerContext;
        }

        /// <summary>
        /// Get all stats related to TFN records.
        /// </summary>
        /// <param name="paging">Paging metadata.</param>
        /// <param name="criteria">Search criteria</param>
        [HttpGet]
        [SupportsPaging(typeof(TFNStatsCriteria))]
        [Authorize(Policy = AuthorisationConfiguration.AUTH_Apprentice_TSL_View)]
        public ActionResult<PagedList<TFNStatsV1>> List([FromQuery] PagingInfo paging, [FromQuery] TFNStatsCriteria criteria)
        {
            paging ??= new PagingInfo();
            paging.SetDefaultSorting("CreatedOn", true);

            var tfnRecords = _tfnStatsRetriever.RetrieveTfnStats(criteria);


            var systemDateTime = _callerContext == null || _callerContext.EffectiveDateTime == null || _callerContext.EffectiveDateTime.Value == DateTime.MinValue
                ? DateTime.Now
                : _callerContext.EffectiveDateTime.Value;

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
