using System;
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
        private const string countrycode = "CNTY";
        private const string languageCode = "LANG";

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

        private async
            Task
            ValidateCode(String CodeName, string codevalue, ValidationExceptionType exception)
        {
            IList<ListCodeResponseV1> countryCode = await referenceDataClient.GetListCodes(CodeName, codevalue, true, true);
            if (!countryCode.Any())
            {
                throw exceptionFactory.CreateValidationException(exception);
            }
        }

        public async Task ValidateAsync(Profile profile)
        {
            if (!string.IsNullOrEmpty(profile?.CountryOfBirthCode))
            {
                await ValidateCode(countrycode, profile.CountryOfBirthCode, ValidationExceptionType.InvalidCountryCode);
            }
            if (!string.IsNullOrEmpty(profile?.LanguageCode))
            {
                await ValidateCode(languageCode, profile.LanguageCode, ValidationExceptionType.InvalidLanguageCode);
            }

            //  return profile;
        }
    }
}