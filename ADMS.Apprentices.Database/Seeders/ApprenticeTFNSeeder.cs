using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using Adms.Shared;
using ADMS.Apprentices.Core.Messages.TFN;
using ADMS.Apprentices.Core.Services;

namespace ADMS.Apprentices.Database.Seeders
{
    #region NotInUse - no seeding at the moment
    public class ApprenticeTFNSeeder : IDataSeeder
    {
        private readonly IRepository repository;
        private readonly IApprenticeTFNCreator ApprenticeTFNCreator;

        public ApprenticeTFNSeeder(IRepository repository,  IApprenticeTFNCreator ApprenticeTFNCreator)
        {
            this.repository = repository;
            this.ApprenticeTFNCreator = ApprenticeTFNCreator;
        }

        public int Order => 100;

        public async Task SeedAsync()
        {

            if (!repository.Retrieve<ApprenticeTFN>().Any())
            {
                await ApprenticeTFNCreator.CreateAsync(new ApprenticeTFNV1
                {
                    ApprenticeId = 1,
                    TaxFileNumber = 123456789
                });
                await ApprenticeTFNCreator.CreateAsync(new ApprenticeTFNV1
                {
                    ApprenticeId = 2,
                    TaxFileNumber = 456564645
                });
            }
        }

    }
    #endregion
}
