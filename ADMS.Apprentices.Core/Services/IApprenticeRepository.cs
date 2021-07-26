using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;

namespace ADMS.Apprentices.Core.Services
{
    public interface IApprenticeRepository
    {
        Task<ICollection<ProfileSearchResultModel>> GetProfilesAsync(ProfileSearchMessage searchMessage);
        Task<ApprenticeIdentitySearchResultModel[]> GetMatchesByIdentityAsync(ApprenticeIdentitySearchCriteriaMessage message);
    }
}