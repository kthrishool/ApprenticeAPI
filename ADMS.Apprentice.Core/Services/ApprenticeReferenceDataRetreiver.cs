using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services
{
    public class ApprenticeReferenceDataRetreiver : IApprenticeReferenceDataRetreiver
    {
        private readonly ITyimsRepository tyimsRepository;
        private readonly IExceptionFactory exceptionFactory;

        public ApprenticeReferenceDataRetreiver(ITyimsRepository tyimsRepository, IExceptionFactory exceptionFactory)
        {
            this.tyimsRepository = tyimsRepository;
            this.exceptionFactory = exceptionFactory;
        }

        public async Task<List<CodeLocalityPostcodesState>> GetLocalityPostcodesStateAsync(string postcode)
        {
            List<CodeLocalityPostcodesState> postCode = await tyimsRepository.GetPostCodeAsync(postcode);
            if (postCode == null)
                throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidPostcode);

            return postCode;
        }
    }
}