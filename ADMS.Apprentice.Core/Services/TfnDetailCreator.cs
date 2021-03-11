using Adms.Shared;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    public class TfnDetailCreator : ITfnDetailCreator
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;


        public TfnDetailCreator (
            IRepository repository,
            ICryptography cryptography
            )
        {
            this.repository = repository;
            this.cryptography = cryptography;
        }

        public async Task<TfnDetail> CreateTfnDetailAsync(TFNV1 message)
        {
            var tfnDetail = new TfnDetail { 
                ApprenticeId = message.ApprenticeId,
                TFN = cryptography.EncryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber),
                Status = TFNStatus.New
            };

            repository.Insert(tfnDetail);
            await repository.SaveAsync();

            tfnDetail.TfnStatusHistories.Add(new TfnStatusHistory
            {
                TfnDetailId = tfnDetail.Id,
                Status = tfnDetail.Status
            });

            return tfnDetail;
        }
    }
}