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

        public ReferenceDataValidator(
            IRepository repository,
            IExceptionFactory exceptionFactory,
            IReferenceDataClient referenceDataClient
        )
        {
            this.repository = repository;
            this.exceptionFactory = exceptionFactory;
            this.referenceDataClient = referenceDataClient;            
        }

        //private async void getCountryOfBirthTables(Profile profile)
        //{
        //    if (!string.IsNullOrEmpty(profile?.CountryOfBirthCode))
        //    {
        //        IList<ListCodeResponseV1> countryCode = await referenceDataClient.GetListCodes("CNTY", profile.CountryOfBirthCode, true, true);
        //        if (!countryCode.Any())
        //        {
        //            throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidCountryCode);
        //        }
        //    }
        //}

        private async Task ValidateCode(String CodeName, string codevalue, ValidationExceptionType exception)
        {
            IList<ListCodeResponseV1> validCodes = await referenceDataClient.GetListCodes(CodeName, codevalue, true, true);
            if (!validCodes.Any())
            {
                throw exceptionFactory.CreateValidationException(exception);
            }
        }

        public async Task ValidateAsync(Profile profile)
        {
            if (!string.IsNullOrEmpty(profile?.CountryOfBirthCode))
            {
                await ValidateCode(CodeTypes.country, profile.CountryOfBirthCode, ValidationExceptionType.InvalidCountryCode);
            }
            if (!string.IsNullOrEmpty(profile?.LanguageCode))
            {
                await ValidateCode(CodeTypes.language, profile.LanguageCode, ValidationExceptionType.InvalidLanguageCode);
            }
            if (!string.IsNullOrEmpty(profile?.HighestSchoolLevelCode))
            {
                await ValidateCode(CodeTypes.schoolLevel, profile.HighestSchoolLevelCode, ValidationExceptionType.InvalidHighestSchoolLevelCode);
            }
            if (!string.IsNullOrEmpty(profile?.LeftSchoolMonthCode))
            {
                await ValidateCode(CodeTypes.month, profile.LeftSchoolMonthCode, ValidationExceptionType.InvalidMonthCode);
            }
        }
    }
}