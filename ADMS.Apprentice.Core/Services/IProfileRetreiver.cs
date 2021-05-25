using System.Linq;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Messages;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IProfileRetreiver
    {
        IQueryable<Profile> RetreiveList();
        IEnumerable<ProfileSearchResultModel> Search(ProfileSearchMessage message);        
    }
}