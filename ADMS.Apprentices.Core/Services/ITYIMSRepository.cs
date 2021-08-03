using System.Threading.Tasks;
using ADMS.Apprentices.Core.TYIMS.Entities;

namespace ADMS.Apprentices.Core.Services
{
    public interface ITYIMSRepository
    {
        Task<Registration> GetCompletedRegistrationsByApprenticeIdAsync(int apprenticeId);
    }
}