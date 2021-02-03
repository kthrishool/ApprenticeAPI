using ADMS.Services.Apprentice.Model;
using ADMS.Services.Infrastructure.Core.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMS.Services.Apprentice.ServiceInterface
{
    /// <remarks />
    public interface IApprenticeService : IService
    {

        /// <remarks />
        Task<IList<RelatedCode>> GetRelatedCodesAsync(RelatedCodeRequest request);

    }
}