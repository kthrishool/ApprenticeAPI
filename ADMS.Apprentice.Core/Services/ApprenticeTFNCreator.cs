using System;
using System.Linq;
using Adms.Shared;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Database;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services
{
    public class ApprenticeTFNCreator : IApprenticeTFNCreator
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IContextRetriever contextRetriever;


        public ApprenticeTFNCreator(
            IRepository repository,
            ICryptography cryptography, 
            IContextRetriever contextRetriever,
            IExceptionFactory exceptionFactory)
        {
            this.repository = repository;
            this.cryptography = cryptography;
            this.contextRetriever = contextRetriever;
            this.exceptionFactory = exceptionFactory;
        }

        public async Task<ApprenticeTFN> CreateAsync(ApprenticeTFNV1 message)
        {
            if (message.ApprenticeId <= 0)
            {
                exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeId);
            }

            if (message.TaxFileNumber <= 0)
            {
                exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidTFN);
            }

            var apprenticeProfile = repository.Get<Profile>(message.ApprenticeId);

            if (apprenticeProfile == null)
            {
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", message.ApprenticeId.ToString());
            }

            
            // Check if TFN is not already added to the apprentice's profile
            
            var tfnEntity = repository
                .Retrieve<ApprenticeTFN>()
                .FirstOrDefault(x => x.ApprenticeId == message.ApprenticeId); // TODO check only Active TFNs once that flag/column is added to the table && !x.InactiveFlag

            if (tfnEntity != null)
            {
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.TFNAlreadyExists);
            }


            var apprenticeTFN = new ApprenticeTFN
            {
                ApprenticeId = message.ApprenticeId,
                TaxFileNumber = cryptography.EncryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber.ToString()),
                StatusCode = TFNStatus.New,
                MessageQueueCorrelationId = Guid.NewGuid(),
                StatusDate = contextRetriever.GetContext().DateTimeContext,
            };

            repository.Insert(apprenticeTFN);
            await repository.SaveAsync();

            return apprenticeTFN;
        }
    }
}