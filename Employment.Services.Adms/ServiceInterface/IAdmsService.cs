using System.Threading.Tasks;
using Employment.Services.Infrastructure.Core.Interface;
using Employment.Services.Adms.Model;
using System.Collections.Generic;

namespace Employment.Services.Adms.ServiceInterface
{
    /// <remarks />
    public interface IAdmsService : IService
    {

        /// <remarks />
        Task<IList<RelatedCode>> GetRelatedCodesAsync(RelatedCodeRequest request);

    }
}