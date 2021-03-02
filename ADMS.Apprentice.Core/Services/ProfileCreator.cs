using System;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared;

namespace ADMS.Apprentice.Core.Services
{
    public class ProfileCreator :  IProfileCreator
    {
        private readonly IRepository repository;

        public ProfileCreator(IRepository repository)
        {
            this.repository = repository;
        }

        public Task<Profile> CreateAsync(ProfileMessage message)
        {            
            var profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                OtherNames = message.OtherNames,
                PreferredName = message.PreferredName,
                BirthDate = message.BirthDate             
            };
            repository.Insert(profile);
            // doesn't need to be async just yet, but it will be once we start looking up TYIMS data etc
            return Task.FromResult(profile);
        }
    }
}