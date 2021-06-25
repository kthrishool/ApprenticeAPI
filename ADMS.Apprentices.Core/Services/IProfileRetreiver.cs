using System.Linq;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Messages;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface IProfileRetreiver
    {
        IQueryable<Profile> RetreiveList();
        ICollection<ProfileSearchResultModel> Search(ProfileSearchMessage message);        
    }
}