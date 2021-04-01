using System;
using System.Linq;
using Adms.Shared;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages.TFN;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Database;
using Adms.Shared.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace ADMS.Apprentice.Core.Services
{
    public class ApprenticeTFNUpdater : IApprenticeTFNUpdater
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IContextRetriever contextRetriever;

        public ApprenticeTFNUpdater(
            IRepository repository,
            ICryptography cryptography, 
            IContextRetriever contextRetriever,
            IExceptionFactory exceptionFactory
            )
        {
            this.repository = repository;
            this.cryptography = cryptography;
            this.contextRetriever = contextRetriever;
            this.exceptionFactory = exceptionFactory;

        }

        public async Task<ApprenticeTFN> SetRevalidate(int apprenticeId)
        {
            var tfnEntity = Get(apprenticeId);

            tfnEntity.StatusCode = TFNStatus.TBVE;
            tfnEntity.StatusDate = contextRetriever.GetContext().DateTimeContext;
            tfnEntity.MessageQueueCorrelationId = Guid.NewGuid();
            tfnEntity.StatusReasonCode = null;

            await repository.SaveAsync();

            return tfnEntity;
        }

        public async Task<ApprenticeTFN> Update(ApprenticeTFNV1 message)
        {
            var tfnEntity = Get(message.ApprenticeId);

            tfnEntity.TaxFileNumber = cryptography.EncryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber.ToString());
            tfnEntity.StatusCode = TFNStatus.TBVE;
            tfnEntity.StatusDate = contextRetriever.GetContext().DateTimeContext;
            tfnEntity.StatusReasonCode = null;
            tfnEntity.MessageQueueCorrelationId = Guid.NewGuid();

            await repository.SaveAsync();

            return tfnEntity;
        }

        public ApprenticeTFN Get(int apprenticeId)
        {
            var tfnEntity = repository
                .Retrieve<ApprenticeTFN>()
                .FirstOrDefault(x => x.ApprenticeId == apprenticeId); // TODO check only Active TFNs once that flag/column is added to the table && !x.InactiveFlag

            if (tfnEntity == null)
            {
                throw exceptionFactory.CreateNotFoundException("TaxFileNumber", apprenticeId.ToString());
            }

            return tfnEntity;
        }
    }
}