using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface IPriorApprenticeshipUpdater
    {
        Task<PriorApprenticeship> Update(int apprenticeId, int priorApprenticeshipId, ProfilePriorApprenticeshipMessage message, Profile profile);
    }
}