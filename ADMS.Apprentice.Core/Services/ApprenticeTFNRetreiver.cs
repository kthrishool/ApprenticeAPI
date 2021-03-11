using Adms.Shared;
using Adms.Shared.Extensions;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    public class ApprenticeTFNRetreiver : IApprenticeTFNRetreiver
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;


        public ApprenticeTFNRetreiver(
            IRepository repository,
            ICryptography cryptography
            )
        {
            this.repository = repository;
            this.cryptography = cryptography;
        }

        public async Task<ApprenticeTFNModel> Get(int id)
        {
            ApprenticeTFN tfnDetail = await repository
                           .Retrieve<ApprenticeTFN>()
                           .GetAsync(id);

            var model = new ApprenticeTFNModel(tfnDetail);

            model.TFN = cryptography.DecryptTFN(model.ApprenticeId.ToString(), model.TFN);

            return model;
        }
    }
}