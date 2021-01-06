using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Employment.Services.Infrastructure.Core.Interface;
using Employment.Services.Adms.Model;

namespace Employment.Services.Adms.Repository
{
    /// <summary>
    /// Repository class
    /// </summary>
    public interface IAdmsRepository : IRepository
    {

        /// <remarks />
        Task<IList<RelatedCode>> GetRelatedCodesAsync(RelatedCodeRequest request);

    }
}