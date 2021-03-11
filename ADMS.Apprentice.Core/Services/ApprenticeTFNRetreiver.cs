using Adms.Shared;
using Adms.Shared.Extensions;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Exceptions;

namespace ADMS.Apprentice.Core.Services
{
    public class ApprenticeTFNRetreiver : IApprenticeTFNRetreiver
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;
        private readonly IExceptionFactory exceptionFactory;


        public ApprenticeTFNRetreiver(
            IRepository repository,
            ICryptography cryptography, 
            IExceptionFactory exceptionFactory)
        {
            this.repository = repository;
            this.cryptography = cryptography;
            this.exceptionFactory = exceptionFactory;
        }

        public async Task<ApprenticeTFNModel> Get(int id)
        {
            var tfnEntity = await repository
                .Retrieve<ApprenticeTFN>()
                .FirstOrDefaultAsync(x => x.ApprenticeId == id);

            if (tfnEntity == null)
            {
                throw exceptionFactory.CreateNotFoundException("TFN", $"apprenticeId {id}");
            }

            var model = new ApprenticeTFNModel(tfnEntity);

            model.TFN = cryptography.DecryptTFN(model.ApprenticeId.ToString(), model.TFN);

            return model;
        }
    }
}