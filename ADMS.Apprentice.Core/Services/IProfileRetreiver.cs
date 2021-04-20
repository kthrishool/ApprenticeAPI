using System.Linq;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IProfileRetreiver
    {
        IQueryable<Profile> RetreiveList(); //(ProfileSearchCriteria criteria);
    }
}