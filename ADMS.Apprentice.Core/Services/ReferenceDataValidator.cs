using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Adms.Shared;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentice.Core.Services
{
    public class ReferenceDataValidator : IReferenceDataValidator
    {
        private readonly IRepository repository;
        private readonly IExceptionFactory exceptionFactory;
        private readonly IReferenceDataClient referenceDataClient;

        public ReferenceDataValidator(
            IRepository repository,
            IExceptionFactory exceptionFactory,
            IReferenceDataClient referenceDataClient
        )
        {
            this.repository = repository;
            this.exceptionFactory = exceptionFactory;
            this.referenceDataClient = referenceDataClient;
            // get apprentice Code tables
        }

        private async void getCountryOfBirthTables(Profile profile)
        {
            if (!string.IsNullOrEmpty(profile?.CountryOfBirthCode))
            {
                IList<ListCodeResponseV1> countryCode = await referenceDataClient.GetListCodes("CNTY", profile.CountryOfBirthCode, true, true);
                if (!countryCode.Any())
                {
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidCountryCode);
                }
            }
        }

        public async Task<Profile> ValidateAsync(Profile profile)
        {
            if (!string.IsNullOrEmpty(profile?.CountryOfBirthCode))
            {
                IList<ListCodeResponseV1> countryCode = await referenceDataClient.GetListCodes("CNTY", profile.CountryOfBirthCode, true, true);
                if (!countryCode.Any())
                {
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidCountryCode);
                }
            }
            //getCountryOfBirthTables(profile);
            return profile;
        }
    }
}