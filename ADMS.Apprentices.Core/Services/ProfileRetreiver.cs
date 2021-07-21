using System.Linq;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared;
using Adms.Shared.Exceptions;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Messages;
using System.Threading.Tasks;
using System.Collections.Generic;
using ADMS.Apprentices.Core.Exceptions;
using System.Diagnostics.CodeAnalysis;
using Adms.Shared.Extensions;
using System;

namespace ADMS.Apprentices.Core.Services
{
    public class ProfileRetreiver : IProfileRetreiver
    {
        private readonly IRepository repository;
        private readonly IApprenticeRepository apprenticeRepository;

        public ProfileRetreiver(
            IRepository repository,
            IApprenticeRepository apprenticeRepository
            )
        {
            this.repository = repository;
            this.apprenticeRepository = apprenticeRepository;
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
            bool noSearchParams = message.ApprenticeID == null && message.Name.IsNullOrEmpty() && message.BirthDate == null && message.USI.IsNullOrEmpty();

            if ( message.Phonenumber?.Length < 8 &&  message.Address.IsNullOrEmpty() && message.EmailAddress.IsNullOrEmpty() && noSearchParams)
                throw AdmsValidationException.Create(ValidationExceptionType.InvalidPhonenumberSearch);

            if (message.EmailAddress?.Length < 4 && message.Address.IsNullOrEmpty() && message.Phonenumber.IsNullOrEmpty() && noSearchParams)
                throw AdmsValidationException.Create(ValidationExceptionType.InvalidEmailSearch);

            if (message.Phonenumber.IsNullOrEmpty() && message.Address.IsNullOrEmpty() && message.EmailAddress.IsNullOrEmpty() && noSearchParams)
                throw AdmsValidationException.Create(ValidationExceptionType.InvalidSearch);

            if (!message.Address.IsNullOrEmpty() && Enum.IsDefined(typeof(StateCode), message.Address.ToUpper()) && message.Phonenumber.IsNullOrEmpty() && message.EmailAddress.IsNullOrEmpty() && noSearchParams)
                throw AdmsValidationException.Create(ValidationExceptionType.InvalidAddressSearch);

            return apprenticeRepository.GetProfilesAsync(message).Result;
        }
    }
}
