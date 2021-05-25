using System.Linq;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared;
using Adms.Shared.Exceptions;
using System.Diagnostics.CodeAnalysis;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Messages;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ADMS.Apprentice.Core.Services
{
    //TODO: implement unit testing
    [ExcludeFromCodeCoverage]
    public class ProfileRetreiver : IProfileRetreiver
    {
        private readonly IRepository repository;
        private readonly IApprenticeRepository apprenticeRepository;
        private readonly IExceptionFactory exceptionFactory;

        public ProfileRetreiver(
            IRepository repository,
            IApprenticeRepository apprenticeRepository,
            IExceptionFactory exceptionFactory)
        {
            this.repository = repository;
            this.apprenticeRepository = apprenticeRepository;
            this.exceptionFactory = exceptionFactory;
        }

        /// <summary>
        /// Returns as list of apprentices based on the search criteria        
        /// </summary>
        public IQueryable<Profile> RetreiveList() 
        {
            IQueryable<Profile> profiles = null;

            profiles = repository.Retrieve<Profile>().Where(x => x.ActiveFlag == true).AsQueryable().Take(500);

            return profiles;
        }

        /// <summary>        
        /// Returns as list of apprentices based on the search Criteria              
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public  IEnumerable<ProfileSearchResultModel> Search(ProfileSearchMessage message)
        {
            return  apprenticeRepository.GetProfilesAsync(message).Result;
        }
    }
}