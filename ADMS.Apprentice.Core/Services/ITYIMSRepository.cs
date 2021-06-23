using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.TYIMS.Entities;

namespace ADMS.Apprentice.Core.Services
{
    public interface ITYIMSRepository
    {
        Task<Registration> GetRegistrationAsync(int registrationId);
    }
}