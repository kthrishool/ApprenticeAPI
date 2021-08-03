using Adms.Shared;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using Adms.Shared.Exceptions;
using Adms.Shared.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core.Services
{
    public class GuardianRetriever : IGuardianRetriever
    {
        private readonly IRepository repository;        

        public GuardianRetriever (IRepository repository)
        {
            this.repository = repository;            
        }

        public async Task<Guardian> GetAsync(int apprenticeId)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            Guardian guardian = profile.Guardian;
            if (guardian == null)
            {
                throw AdmsNotFoundException.Create("Apprentice guardian", $"ApprenticeId {profile.Id}");
            }
            return guardian;
        }
    }
}