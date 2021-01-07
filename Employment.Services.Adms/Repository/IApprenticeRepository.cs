using ADMS.Services.Apprentice.Model;
using Employment.Services.Infrastructure.Core.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMS.Services.Apprentice.Repository
{
    /// <summary>
    /// Repository class
    /// </summary>
    public interface IApprenticeRepository : IRepository
    {

        /// <remarks />
        Task<IList<RelatedCode>> GetRelatedCodesAsync(RelatedCodeRequest request);

    }
}