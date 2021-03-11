using System;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared;
using System.Linq;

namespace ADMS.Apprentice.Core.Services
{
    public class ProfileCreator :  IProfileCreator
    {
        private readonly IRepository repository;
        private readonly IProfileValidator profileValidator;

        public ProfileCreator(IRepository repository,
            IProfileValidator profileValidator)
        {
            this.repository = repository;
            this.profileValidator = profileValidator;
        }

        public async Task<Profile> CreateAsync(ProfileMessage message)
        {
           
            var profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                OtherNames = message.OtherNames,
                PreferredName = message.PreferredName,
                BirthDate = message.BirthDate,
                EmailAddress =  message.EmailAddress,
                ProfileTypeCode = Enum.IsDefined(typeof(ProfileType), message?.ProfileType)?message.ProfileType:null
            };
            await profileValidator.ValidateAsync(profile);
            repository.Insert(profile);
            // doesn't need to be async just yet, but it will be once we start looking up TYIMS data etc
            return profile;
        }
    }
}