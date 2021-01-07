using System.Threading.Tasks;
using Employment.Services.Infrastructure.Core.Logging;
using Employment.Services.Infrastructure.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Employment.Services.Infrastructure.WebApi.Documentation;
using Employment.Services.Infrastructure.Contract;
using ADMS.Services.Apprentice.Contract;
using ADMS.Services.Apprentice.Model;
using ADMS.Services.Apprentice.ServiceInterface;
#if NETFRAMEWORK
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Employment.Services.Infrastructure.WebApi.Compatibility;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
#endif


namespace ADMS.Services.Apprentice.WebApi.Controllers
{
    /// <remarks />
    [Public]
#if !NETFRAMEWORK
    [ApiVersion(Version = "1", Latest = "1")]
    [Route("api/v1/RelatedCodes")]
    [Route("api/RelatedCodes")]
    [Produces("application/json")]
    [Consumes("application/json")]
#endif
    public class RelatedCodesV1Controller : InfrastructureController<IApprenticeService> {

#if !NETFRAMEWORK
        /// <remarks />
        public RelatedCodesV1Controller(IHttpContextAccessor contextAccessor) : base(contextAccessor) { }
#endif


        /// <summary>
        /// Get Related Codes.
        /// </summary>
        /// <param name="RelatedCodeType">The type of related code.</param>
        /// <param name="SearchCode">The code to search for.</param>
        /// <param name="DominantSearch">Whether it is a dominant search (default is true).</param>
        /// <param name="CurrentCodesOnly">Whether to use current codes only (default is true).</param>
        /// <param name="ExactLookup">Whether to do an exact lookup (default is false).</param>
        /// <param name="MaxRows">The maximum number of rows to return (default is unlimited).</param>
        /// <param name="RowPosition">The row position.</param>
        /// <param name="CurrentDate">The current date for the purposes of including/excluding end-dated codes.</param>
        /// <param name="EndDateInclusive">Whether to check current dates based on GEN logic (inclusive end date).</param>
        /// <returns></returns>
        [HttpGet]
#if NETFRAMEWORK
      [ResponseType(typeof(IList<RelatedCodeResponseV1>))]
        public async Task<IHttpActionResult> 
#else
        public async Task<ActionResult<WrappedHttpResponseBody<IList<RelatedCodeResponseV1>>>>
#endif
        GetRelatedCodes(string RelatedCodeType, string SearchCode = "", bool DominantSearch = true, bool CurrentCodesOnly = true, bool ExactLookup = false, int MaxRows = 0, int RowPosition = 0, string CurrentDate = "", bool? EndDateInclusive = null)
        {
            try
            {
                Logger.Default.Debug(this.HttpContextAccess, 0, "Request for RelatedCodeType: " + RelatedCodeType);
                RelatedCodeRequest request = new RelatedCodeRequest();
                request.CurrentCodesOnly = CurrentCodesOnly;
                request.CurrentDate = string.IsNullOrEmpty(CurrentDate) ? (DateTime?)null : DateTime.Parse(CurrentDate);
                request.DominantSearch = DominantSearch;
                request.ExactLookup = ExactLookup;
                request.MaxRows = MaxRows;
                request.RelatedCodeType = RelatedCodeType;
                request.RowPosition = RowPosition;
                request.SearchCode = SearchCode;
                request.EndDateInclusive = EndDateInclusive;
                IList<RelatedCode> result = await ExecuteServiceMethodAsync(request, Service.GetRelatedCodesAsync);
                IList<RelatedCodeResponseV1> response = result.Select(i => AdmsMaps.MapToRelatedCodeResponseV1(i)).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.Default.Error(this.HttpContextAccess, 0, ex);
                throw;
            }
        }
    }
}