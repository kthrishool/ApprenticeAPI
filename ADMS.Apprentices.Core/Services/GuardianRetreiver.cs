using Adms.Shared;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using Adms.Shared.Exceptions;
using Adms.Shared.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentices.Core.Services
{
    public class GuardianRetreiver : IGuardianRetreiver
    {
        private readonly IRepository repository;        
        private readonly IExceptionFactory exceptionFactory;

        public GuardianRetreiver (IRepository repository, IExceptionFactory exceptionFactory)        
        {
            this.repository = repository;            
            this.exceptionFactory = exceptionFactory;
        }

        public async Task<Guardian> GetAsync(int apprenticeId)
        {
            Profile profile = await repository.GetAsync<Profile>(apprenticeId, true);
            Guardian guardian = profile.Guardian;
            if (guardian == null)
            {
                throw exceptionFactory.CreateNotFoundException("Apprentice guardian", $"ApprenticeId {profile.Id}");
            }
            return guardian;
        }
    }
}