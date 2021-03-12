using Adms.Shared;
using Adms.Shared.Extensions;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Adms.Shared.Exceptions;
using Adms.Shared.Database;
using ADMS.Services.Infrastructure.Core.Exceptions;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services
{
    public class ApprenticeTFNRetreiver : IApprenticeTFNRetreiver
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IContextRetriever contextRetriever;

        public ApprenticeTFNRetreiver(
            IRepository repository,
            ICryptography cryptography, 
            IExceptionFactory exceptionFactory,
            IContextRetriever contextRetriever
            )
        {
            this.repository = repository;
            this.cryptography = cryptography;
            this.exceptionFactory = exceptionFactory;
            this.contextRetriever = contextRetriever;
        }

        public async Task<ApprenticeTFNModel> Get(int id)
        {
            var tfnEntity = await repository
                .Retrieve<ApprenticeTFN>()
                .FirstOrDefaultAsync(x => x.ApprenticeId == id);

            if (tfnEntity == null)
            {
                throw new ValidationException(contextRetriever.GetContext(), new ADMS.Services.Infrastructure.Core.Validation.ValidationError ("TFN", $"apprenticeId {id}"));
            }

            var model = new ApprenticeTFNModel(tfnEntity);

            model.TFN = cryptography.DecryptTFN(model.ApprenticeId.ToString(), model.TFN);

            return model;
        }
    }
}