using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentice.Core.Messages.TFN;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.WebApi;
using ADMS.Services.Infrastructure.WebApi.Documentation;
using Adms.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Filters;
using Adms.Shared.Paging;
using ADMS.Apprentice.Core.Models.TFN;
using Adms.Shared.Extensions;
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
        private readonly ITFNStatsRetriever _tfnStatsRetriever;

        public TFNStatsController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper,
            ITFNStatsRetriever tfnStatsRetriever) : base(contextAccessor)
        {
            this.repository = repository;
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
            paging.SetDefaultSorting("ApprenticeId", true);

            var tfnRecords = _tfnStatsRetriever.RetrieveTfnStats(criteria);

            //// REMOVE this as it is not good for performance; Handle sorting on NumberOfDaysSinceTheMismatch
            //if (!string.IsNullOrWhiteSpace(paging.SortBy) && paging.SortBy.Equals("NumberOfDaysSinceTheMismatch"))
            //{
            //    foreach (ApprenticeTFN apprenticeTFN in tfnRecords)
            //    {
            //        apprenticeTFN.NumberOfDaysSinceTheMismatch = 
            //            apprenticeTFN.StatusCode == TFNStatus.NOCH 
            //                ?
            //                GetNumberOfDaysSinceTheMismatch(apprenticeTFN.StatusDate, systemDateTime)
            //                :
            //                null;
            //    }

            //    var list = paging.Descending ? tfnRecords.ToList().OrderByDescending(x => x.NumberOfDaysSinceTheMismatch) : tfnRecords.ToList().OrderBy(x => x.NumberOfDaysSinceTheMismatch);
            //    tfnRecords = list.AsQueryable();
            //    paging.SortBy = "";
            //}

            var systemDateTime = Context == null || Context.DateTimeContext == DateTime.MinValue
                ? DateTime.Now
                : Context.DateTimeContext;

            PagedList<ApprenticeTFN> tfnPagedList = _pagingHelper.ToPagedList(tfnRecords, paging);

            
            return Ok(new PagedList<TFNStatsV1>(tfnPagedList, tfnPagedList.Results.Map(x => new TFNStatsV1(
               x.ApprenticeId,
               x.Profile?.FirstName + " " + x.Profile?.Surname,
               x.Profile?.BirthDate,
               x.StatusDate,
               x.CreatedOn,
               x.StatusCode.ToString(),
               x.StatusCode == TFNStatus.NOCH ? GetNumberOfDaysSinceTheMismatchAsString(x.StatusDate, systemDateTime) : "-"
               ))));
        }

        
        private int GetNumberOfDaysSinceTheMismatch(DateTime tfnStatusDate, DateTime systemDateTime)
        {
            return systemDateTime.Subtract(tfnStatusDate).Days > 0 ? systemDateTime.Subtract(tfnStatusDate).Days : 0;
        }

        private string GetNumberOfDaysSinceTheMismatchAsString(DateTime tfnStatusDate, DateTime systemDateTime)
        {
            var number = GetNumberOfDaysSinceTheMismatch(tfnStatusDate, systemDateTime);
            return number > 0 ? number.ToString() : "<1";
        }


        


    }
}