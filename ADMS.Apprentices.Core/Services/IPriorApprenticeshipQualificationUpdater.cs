using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface IPriorApprenticeshipQualificationUpdater
    {
        Task<PriorApprenticeshipQualification> Update(int apprenticeId, int priorApprenticeshipQualificationId, PriorApprenticeshipQualificationMessage message, Profile profile);
    }
}