﻿using System;
using System.Linq;
using Adms.Shared;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Database;
using Adms.Shared.Exceptions;
using Adms.Shared.Helpers;
using Adms.Shared.Services;

namespace ADMS.Apprentices.Core.Services
{
    public class ApprenticeTFNCreator : IApprenticeTFNCreator
    {
        private readonly IRepository repository;
        private readonly IRepository isolatedRepository;
        private readonly ICryptography cryptography;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IContextRetriever contextRetriever;
        private readonly IServiceBusEventHelper serviceBusEventHelper;


        public ApprenticeTFNCreator(
            IRepository repository,
            ICryptography cryptography, 
            IContextRetriever contextRetriever,
            IExceptionFactory exceptionFactory,
            IRepository isolatedRepository,
            IServiceBusEventHelper serviceBusEventHelper)
        {
            this.repository = repository;
            this.cryptography = cryptography;
            this.contextRetriever = contextRetriever;
            this.exceptionFactory = exceptionFactory;
            this.isolatedRepository = isolatedRepository;
            this.serviceBusEventHelper = serviceBusEventHelper;
        }

        public async Task<ApprenticeTFN> CreateAsync(ApprenticeTFNV1 message)
        {
            if (message.ApprenticeId <= 0)
            {
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidApprenticeId);
            }

            if (message.TaxFileNumber <= 0)
            {
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidTFN);
            }

            var apprenticeProfile = repository.Get<Profile>(message.ApprenticeId);

            if (apprenticeProfile == null)
            {
                throw exceptionFactory.CreateNotFoundException("Apprentice Profile", message.ApprenticeId.ToString());
            }

            
            // Check if TFN is not already added to the apprentice's profile
            
            var tfnEntity = repository
                .Retrieve<ApprenticeTFN>()
                .FirstOrDefault(x => x.ApprenticeId == message.ApprenticeId);

            if (tfnEntity != null)
            {
                isolatedRepository.Insert(serviceBusEventHelper.GetServiceBusEvent(new { tfnEntity.ApprenticeId }, "TFNRecordingFailed"));
                isolatedRepository.Save();

                throw exceptionFactory.CreateValidationException(ValidationExceptionType.TFNAlreadyExists);
            }

            var apprenticeTFN = new ApprenticeTFN
            {
                ApprenticeId = message.ApprenticeId,
                TaxFileNumber = cryptography.EncryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber.ToString()),
                StatusCode = TFNStatus.TBVE,
                MessageQueueCorrelationId = Guid.NewGuid(),
                StatusDate = contextRetriever.GetContext().DateTimeContext,
            };

            repository.Insert(apprenticeTFN);
            repository.Insert(serviceBusEventHelper.GetServiceBusEvent(new { apprenticeTFN.ApprenticeId }, "TFNRecorded"));

            await repository.SaveAsync();
            
            return apprenticeTFN;
        }
    }
}