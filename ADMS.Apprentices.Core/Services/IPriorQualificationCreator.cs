using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface IPriorQualificationCreator
    {
        Task<PriorQualification> CreateAsync(int apprenticeId, ProfilePriorQualificationMessage message);
    }
}