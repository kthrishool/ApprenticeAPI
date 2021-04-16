using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public class ProfileUpdater : IProfileUpdater
    {
        private readonly IProfileUpdater profileUpdater;
        public ProfileUpdater(IProfileUpdater profileUpdater)
        {
            this.profileUpdater = profileUpdater;
        }

        public void Update(Profile profile, ProfileMessage message)
        { }
    }
}