using Adms.Shared;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Models;
using Adms.Shared.Exceptions;
using Adms.Shared.Database;
using System.Linq;

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
            IExceptionFactory exceptionFactory
            )
        {
            this.repository = repository;
            this.cryptography = cryptography;
            this.exceptionFactory = exceptionFactory;
        }

        public ApprenticeTFNModel Get(int id)
        {
            var r = repository
                .Retrieve<ApprenticeTFN>();
            var tfnEntity =    r.FirstOrDefault(x => x.ApprenticeId == id);

            if (tfnEntity == null)
            {
                throw exceptionFactory.CreateNotFoundException("TaxFileNumber", $"ApprenticeId {id}");
            }

            var model = new ApprenticeTFNModel(tfnEntity);
            model.TaxFileNumber = cryptography.DecryptTFN(model.ApprenticeId.ToString(), model.TaxFileNumber);

            return model;
        }
    }
}