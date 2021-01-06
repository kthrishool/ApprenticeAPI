using System.Threading.Tasks;
using Employment.Services.Infrastructure.Core.Data;
using Employment.Services.Infrastructure.Core.Delegates;
using Employment.Services.Infrastructure.Core.Interface;
using Employment.Services.Infrastructure.Service;
using Employment.Services.Adms.Business;
using Employment.Services.Adms.Model;
using Employment.Services.Adms.ServiceInterface;
using System.Collections.Generic;
using System;

namespace Employment.Services.Adms.Service
{
    /// <remarks />
    public class AdmsService : ServiceBase, IAdmsService
    {
        private readonly AdmsBusiness _AdmsBusiness;

        /// <remarks />
        public AdmsService(IContext context) : base(context)
        {
            _AdmsBusiness = new AdmsBusiness(context);
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
