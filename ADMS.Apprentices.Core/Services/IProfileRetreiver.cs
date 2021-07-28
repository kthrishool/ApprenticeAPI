using System.Linq;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Messages;
using System.Threading.Tasks;
using System.Collections.Generic;
using Adms.Shared.Paging;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface IProfileRetreiver
    {
        Task<PagedList<ProfileListModel>> RetreiveList(PagingInfo paging, ProfileSearchMessage message);
        Task<ICollection<ProfileSearchResultModel>> Search(ProfileSearchMessage message);        
    }
}