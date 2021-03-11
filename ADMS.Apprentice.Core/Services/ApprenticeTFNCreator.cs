using Adms.Shared;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.Services
{
    public class ApprenticeTFNCreator : IApprenticeTFNCreator
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;


        public ApprenticeTFNCreator(
            IRepository repository,
            ICryptography cryptography
            )
        {
            this.repository = repository;
            this.cryptography = cryptography;
        }

        public async Task<ApprenticeTFN> CreateAsync(ApprenticeTFNV1 message)
        {
            var ApprenticeTFN = new ApprenticeTFN
            {
                ApprenticeId = message.ApprenticeId,
                TaxFileNumber = cryptography.EncryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber),
                StatusCode = TFNStatus.New
            };

            repository.Insert(ApprenticeTFN);
            await repository.SaveAsync();

            return ApprenticeTFN;
        }
    }
}