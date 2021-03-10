using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared;
using Adms.Shared.Extensions;
using Adms.Shared.Helpers;
using LumenWorks.Framework.IO.Csv;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;

namespace ADMS.Apprentice.Database.Seeders
{
    #region NotInUse - no seeding at the moment
    public class TfnDetailSeeder : IDataSeeder
    {
        private readonly IRepository repository;
        private readonly ITfnDetailCreator tfnDetailCreator;

        public TfnDetailSeeder(IRepository repository,  ITfnDetailCreator tfnDetailCreator)
        {
            this.repository = repository;
            this.tfnDetailCreator = tfnDetailCreator;
        }

        public int Order => 100;

        public async Task SeedAsync()
        {

            if (!repository.Retrieve<TfnDetail>().Any())
            {
                await tfnDetailCreator.CreateTfnDetailAsync(new TfnCreateMessage
                {
                    ApprenticeId = 1,
                    TFN = "123456789",
                    Status = TfnStatus.New
                });
                await tfnDetailCreator.CreateTfnDetailAsync(new TfnCreateMessage
                {
                    ApprenticeId = 2,
                    TFN = "123456789",
                    Status = TfnStatus.New
                });
            }
        }

    }
    #endregion
}
