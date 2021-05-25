using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{    
    public interface IApprenticeRepository
    {
        Task<ICollection<ProfileSearchResultModel>> GetProfilesAsync(ProfileSearchMessage searchMessage);        
    }
}