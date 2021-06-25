using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentices.Core.TYIMS.Entities;

namespace ADMS.Apprentices.Core.Services
{
    public interface ITYIMSRepository
    {
        Task<Registration> GetRegistrationAsync(int registrationId);
    }
}