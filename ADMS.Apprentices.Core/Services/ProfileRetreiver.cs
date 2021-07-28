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
using Adms.Shared.Paging;

namespace ADMS.Apprentices.Core.Services
{
    public class ProfileRetreiver : IProfileRetreiver
    {
        private readonly IRepository repository;
        private readonly IApprenticeRepository apprenticeRepository;
        private readonly IPagingHelper pagingHelper;

        public ProfileRetreiver(
            IRepository repository,
            IApprenticeRepository apprenticeRepository,
            IPagingHelper pagingHelper
            )
        {
            this.repository = repository;
            this.apprenticeRepository = apprenticeRepository;
            this.pagingHelper = pagingHelper;
        }

        /// <summary>
        /// Returns as list of apprentices       
        /// </summary>        
        public async Task<PagedList<ProfileListModel>> RetreiveList(PagingInfo paging, ProfileSearchMessage message) 
        {
            PagedList<ProfileListModel> pagedList;
            IEnumerable<ProfileListModel> models;            

            if (message.ApprenticeID == null && message.BirthDate == null && message.Name.IsNullOrEmpty() && message.EmailAddress.IsNullOrEmpty() &&
                message.Phonenumber.IsNullOrEmpty() && message.USI.IsNullOrEmpty() && message.Address.IsNullOrEmpty())
            {
                paging.SetDefaultSorting("id", true);
                PagedList<Profile> profiles = await pagingHelper.ToPagedListAsync(repository.Retrieve<Profile>().Where(x => x.ActiveFlag == true), paging);
                models = profiles.Results.Map(a => new ProfileListModel(a));
                pagedList = new PagedList<ProfileListModel>(profiles, models);
            }
            else
            {
                paging.SetDefaultSorting("ScoreValue", true);
                PagedInMemoryList<ProfileSearchResultModel> profiles = pagingHelper.ToPagedInMemoryList(await Search(message), paging);
                models = profiles.Results.Map(a => new ProfileListModel(a));
                pagedList = new PagedList<ProfileListModel>(profiles, models);
            }

            return pagedList;
        }

        /// <summary>        
        /// Returns as list of apprentices based on the search Criteria              
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ICollection<ProfileSearchResultModel>> Search(ProfileSearchMessage message)
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

            return await apprenticeRepository.GetProfilesAsync(message);
        }
    }
}
