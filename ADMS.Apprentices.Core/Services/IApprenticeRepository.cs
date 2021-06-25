using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core.Services
{    
    public interface IApprenticeRepository
    {
        Task<ICollection<ProfileSearchResultModel>> GetProfilesAsync(ProfileSearchMessage searchMessage);        
    }
}