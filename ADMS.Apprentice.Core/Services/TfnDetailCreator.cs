using Adms.Shared;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    public class TfnDetailCreator : ITfnDetailCreator
    {
        private readonly IRepository repository;
 
        public TfnDetailCreator (
            IRepository repository
            )
        {
            this.repository = repository;
        }

        public async Task<TfnDetail> CreateTfnDetailAsync(TfnCreateMessage message)
        {
            var tfnDetail = new TfnDetail { 
                ApprenticeId = message.ApprenticeId,
                TFN = message.TFN,
                Status = TfnStatus.New
            };


            tfnDetail.TfnStatusHistories.Add(new TfnStatusHistory
            {
                TfnDetailId = tfnDetail.Id,
                Status = tfnDetail.Status
            });

            await Task.Run(()=> repository.Insert(tfnDetail));
            return tfnDetail;
        }
    }
}