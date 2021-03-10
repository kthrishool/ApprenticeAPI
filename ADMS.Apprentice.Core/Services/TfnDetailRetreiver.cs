using Adms.Shared;
using Adms.Shared.Extensions;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    public class TfnDetailRetreiver : ITfnDetailRetreiver
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;


        public TfnDetailRetreiver(
            IRepository repository,
            ICryptography cryptography
            )
        {
            this.repository = repository;
            this.cryptography = cryptography;
        }

        public async Task<TfnDetailModel> Get(int id)
        {
            TfnDetail tfnDetail = await repository
                           .Retrieve<TfnDetail>()
                           .GetAsync(id);

            var model = new TfnDetailModel(tfnDetail);

            model.TFN = cryptography.DecryptTFN(model.ApprenticeId.ToString(), model.TFN);

            return model;
        }
    }
}