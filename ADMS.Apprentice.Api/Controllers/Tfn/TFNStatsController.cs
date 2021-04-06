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

        public TFNStatsController(
            IHttpContextAccessor contextAccessor,
            IRepository repository,
            IPagingHelper pagingHelper) : base(contextAccessor)
        {
            this.repository = repository;
            _pagingHelper = pagingHelper;
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

            IQueryable<ApprenticeTFN> tfnRecords = null;

            if (criteria != null && GetStatusCodeFromCriteria(criteria.StatusCode, out TFNStatus? tfnStatus) && tfnStatus != null)
                tfnRecords = repository.Retrieve<ApprenticeTFN>().Include(x => x.Profile).Where(x => x.StatusCode == tfnStatus).AsQueryable();
            else
                tfnRecords = repository.Retrieve<ApprenticeTFN>().Include(x => x.Profile).AsQueryable();

            // Apply filter
            if (criteria != null && !string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                tfnRecords = tfnRecords.Where(x =>
                    x.ApprenticeId.ToString() == criteria.Keyword
                     ||
                        (x.Profile != null
                         &&
                         (x.Profile.FirstName.ToLower().Contains(criteria.Keyword.ToLower())
                          ||
                          x.Profile.Surname.ToLower().Contains(criteria.Keyword.ToLower())
                          ||
                          (x.Profile.BirthDate.Day + "/" + x.Profile.BirthDate.Month + "/" + x.Profile.BirthDate.Year).Equals(criteria.Keyword)
                        //string.Format("{0:d/M/yyyy}",x.Profile.BirthDate).Equals(criteria.Keyword)
                        //DateTime.ToString() is not accepted by Linq so you could use x.Profile.BirthDate.Date+"/"+
                        ))
                    );
            }

            PagedList<ApprenticeTFN> tfnPagedList = _pagingHelper.ToPagedList(tfnRecords, paging);

            var model = new List<TFNStatsModel>(tfnRecords.Select(x => (TFNStatsModel)x));

            var systemDateTime = Context == null || Context.DateTimeContext == DateTime.MinValue ?
                DateTime.Now
                : Context.DateTimeContext
                ;

            return Ok(new PagedList<TFNStatsV1>(tfnPagedList, tfnPagedList.Results.Map(x => new TFNStatsV1(
               x.ApprenticeId,
               x.Profile?.FirstName + " " + x.Profile?.Surname,
               x.Profile?.BirthDate,
               x.StatusDate,
               x.CreatedOn,
               x.StatusCode.ToString(),
               x.StatusCode == TFNStatus.NOCH ? (systemDateTime.Subtract(x.StatusDate).Days > 0 ? systemDateTime.Subtract(x.StatusDate).Days.ToString() : "<1") : "-"
               ))));
        }

        private bool GetStatusCodeFromCriteria(string statusCode, out TFNStatus? tfnStatus)
        {
            tfnStatus = null;

            if (string.IsNullOrWhiteSpace(statusCode))
                return false;

            if (statusCode.Equals(TFNStatus.MTCH.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = TFNStatus.MTCH;
            else if (statusCode.Equals(TFNStatus.NOCH.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = TFNStatus.NOCH;
            else if (statusCode.Equals(TFNStatus.SBMT.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = TFNStatus.SBMT;
            else if (statusCode.Equals(TFNStatus.TBVE.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = TFNStatus.TBVE;
            else if (statusCode.Equals(TFNStatus.TERR.ToString(), StringComparison.InvariantCultureIgnoreCase))
                tfnStatus = TFNStatus.TERR;
            else
                return false;
            
            return true;
        }


    }
}