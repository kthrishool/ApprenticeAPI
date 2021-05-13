using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public interface IProfileUpdater
    {
        Task<Profile> Update(Profile profile, UpdateProfileMessage message);
        void Update(Profile profile, AdminUpdateMessage message);
        void UpdateDeceasedFlag(Profile profile, bool deceased);

        void UpdateCRN(Profile profile, ServiceAustraliaUpdateMessage message);
    }
}