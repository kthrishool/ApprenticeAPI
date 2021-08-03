using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.Messages;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface IPriorApprenticeshipCreator
    {
        Task<PriorApprenticeship> CreateAsync(int apprenticeId, ProfilePriorApprenticeshipMessage message);
    }
}