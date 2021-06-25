using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Helpers;
using LumenWorks.Framework.IO.Csv;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;

namespace ADMS.Apprentices.Database.Seeders
{
    #region NotInUse - no seeding at the moment
    public class ProfileSeeder : IDataSeeder
    {
        private readonly IRepository repository;
        private readonly IProfileCreator profileCreator;

        public ProfileSeeder(IRepository repository, IProfileCreator profileCreator)
        {
            this.repository = repository;
            this.profileCreator = profileCreator;
        }

        public int Order => 10;

        public async Task SeedAsync()
        {

            if (!repository.Retrieve<Profile>().Any())
            {
                await profileCreator.CreateAsync(new ProfileMessage
                {
                    Surname = "Smith",
                    FirstName = "Sue",
                    BirthDate=new DateTime(1988,3,3),
                    OtherNames = "Sally",
                    PreferredName = "Sam"
                });

                await profileCreator.CreateAsync(new ProfileMessage
                {
                    Surname = "Jones",
                    FirstName = "John",
                    BirthDate = new DateTime(1988, 3, 3),
                    OtherNames = "James",
                    PreferredName = "Jack"
                });
            
            }
        }

    }
    #endregion
}
