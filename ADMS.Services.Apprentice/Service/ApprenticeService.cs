using ADMS.Services.Apprentice.Business;
using ADMS.Services.Apprentice.Model;
using ADMS.Services.Apprentice.ServiceInterface;
using ADMS.Services.Infrastructure.Core.Data;
using ADMS.Services.Infrastructure.Core.Interface;
using ADMS.Services.Infrastructure.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMS.Services.Apprentice.Service
{
    /// <remarks />
    public class ApprenticeService : ServiceBase, IApprenticeService
    {
        private readonly ApprenticeBusiness _AdmsBusiness;

        /// <remarks />
        public ApprenticeService(IContext context) : base(context)
        {
            _AdmsBusiness = new ApprenticeBusiness(context);
        }


        /// <remarks />
        public async Task<IList<RelatedCode>> GetRelatedCodesAsync(RelatedCodeRequest request)
        {
            return await ExecuteBusinessMethodAsync(TransactionIsolationLevel.RepeatableRead, false, request, _GetRelatedCodesAsync);
        }
        /// <remarks />
        protected async Task<IList<RelatedCode>> _GetRelatedCodesAsync(RelatedCodeRequest request)
        {
            return await _AdmsBusiness.GetRelatedCodesAsync(request);
        }

    }
}
