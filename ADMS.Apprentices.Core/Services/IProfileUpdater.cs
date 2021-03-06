using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using Adms.Shared.Attributes;
using System;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public interface IProfileUpdater
    {
        Task<Profile> Update(Profile profile, UpdateProfileMessage message);
        void Update(Profile profile, AdminUpdateMessage message);
        void UpdateDeceasedFlag(Profile profile, bool deceased, DateTime? deceasedDate);
        void UpdateCRN(Profile profile, ServiceAustraliaUpdateMessage message);
    }
}