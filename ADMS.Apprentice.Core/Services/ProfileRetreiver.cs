using System.Linq;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Messages;
using System.Threading.Tasks;
using System.Collections.Generic;
using ADMS.Apprentice.Core.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace ADMS.Apprentice.Core.Services
{
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
        /// Returns as list of apprentices       
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
        public ICollection<ProfileSearchResultModel> Search(ProfileSearchMessage message)
        {       
            if ( message.Phonenumber?.Length < 8 && message.FirstName == null && message.Surname == null && message.OtherNames == null && 
                message.BirthDate == null && message.Address == null && message.EmailAddress == null && message.USI == null)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidPhonenumberSearch);

            if (message.EmailAddress?.Length < 4 && message.FirstName == null && message.Surname == null && message.OtherNames == null &&
                message.BirthDate == null && message.Address == null && message.Phonenumber == null && message.USI == null)
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidEmailSearch);

            return apprenticeRepository.GetProfilesAsync(message).Result;
        }
    }
}