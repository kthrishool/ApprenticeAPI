using Adms.Shared;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using Adms.Shared.Exceptions;
using Adms.Shared.Services;
using System.Linq;

namespace ADMS.Apprentices.Core.Services
{
    public class ApprenticeTFNRetreiver : IApprenticeTFNRetreiver
    {
        private readonly IRepository repository;
        private readonly ICryptography cryptography;

        public ApprenticeTFNRetreiver(
            IRepository repository,
            ICryptography cryptography 
            )
        {
            this.repository = repository;
            this.cryptography = cryptography;
        }

        public ApprenticeTFNModel Get(int id)
        {
            var r = repository
                .Retrieve<ApprenticeTFN>();
            var tfnEntity =    r.FirstOrDefault(x => x.ApprenticeId == id);

            if (tfnEntity == null)
            {
                throw AdmsNotFoundException.Create("TaxFileNumber", $"ApprenticeId {id}");
            }

            var model = new ApprenticeTFNModel(tfnEntity);
            model.TaxFileNumber = cryptography.DecryptTFN(model.ApprenticeId.ToString(), model.TaxFileNumber);

            return model;
        }
    }
}