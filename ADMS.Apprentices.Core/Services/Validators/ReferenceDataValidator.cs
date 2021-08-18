using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.HttpClients.ReferenceDataApi;
using Adms.Shared;
using Adms.Shared.Extensions;

namespace ADMS.Apprentices.Core.Services.Validators
{
    public class ReferenceDataValidator : IReferenceDataValidator
    {
        private readonly IRepository repository;
        private readonly IReferenceDataClient referenceDataClient;

        public ReferenceDataValidator(
            IRepository repository,
            IReferenceDataClient referenceDataClient
        )
        {
            this.repository = repository;
            this.referenceDataClient = referenceDataClient;
        }


        private async Task ValidateCodeAsync(ValidationExceptionBuilder exceptionBuilder, String CodeName, string codevalue, ValidationExceptionType exception)
        {
            IList<ListCodeResponseV1> validCodes = await referenceDataClient.GetListCodes(CodeName, codevalue, true, true);
            if (!validCodes.Any())
            {
                exceptionBuilder.AddException(exception);
            }
        }

        public async Task<ValidationExceptionBuilder> ValidateAsync(Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();
            var tasks = new List<Task>();

            if (!string.IsNullOrEmpty(profile.CountryOfBirthCode))
            {
                tasks.Add(ValidateCodeAsync(exceptionBuilder, CodeTypes.Country, profile.CountryOfBirthCode, ValidationExceptionType.InvalidCountryCode));
            }
            if (!string.IsNullOrEmpty(profile.LanguageCode))
            {
                tasks.Add(ValidateCodeAsync(exceptionBuilder, CodeTypes.Language, profile.LanguageCode, ValidationExceptionType.InvalidLanguageCode));
            }
            if (!string.IsNullOrEmpty(profile.HighestSchoolLevelCode))
            {
                tasks.Add(ValidateCodeAsync(exceptionBuilder, CodeTypes.SchoolLevel, profile.HighestSchoolLevelCode, ValidationExceptionType.InvalidHighestSchoolLevelCode));
            }
            if (!string.IsNullOrEmpty(profile.IndigenousStatusCode))
            {
                tasks.Add(ValidateCodeAsync(exceptionBuilder, CodeTypes.IndigenousStatusCode, profile.IndigenousStatusCode, ValidationExceptionType.InvalidIndigenousStatusCode));
            }
            if (!string.IsNullOrEmpty(profile.CitizenshipCode))
            {
                tasks.Add(ValidateCodeAsync(exceptionBuilder, CodeTypes.CitizenshipCode, profile.CitizenshipCode, ValidationExceptionType.InvalidCitizenshipCode));
            }
            if (!string.IsNullOrEmpty(profile.NotProvidingUSIReasonCode))
            {
                tasks.Add(ValidateCodeAsync(exceptionBuilder, CodeTypes.USIExemptionCode, profile.NotProvidingUSIReasonCode, ValidationExceptionType.InvalidNotProvidingUSIReasonCode));
            }
            await tasks.WaitAndThrowAnyExceptionFound();
            return exceptionBuilder;
        }

        public async Task<ValidationExceptionBuilder> ValidatePriorApprenticeshipQualificationsAsync(PriorApprenticeshipQualification priorApprenticeship)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();

            if (!priorApprenticeship.CountryCode.IsNullOrEmpty())
            {
                await ValidateCodeAsync(exceptionBuilder, CodeTypes.Country, priorApprenticeship.CountryCode, ValidationExceptionType.InvalidPriorApprenticeshipCountryCode);
                if (priorApprenticeship.CountryCode == "1101")
                {
                    if (priorApprenticeship.StateCode.IsNullOrEmpty())
                        exceptionBuilder.AddException(ValidationExceptionType.InvalidPriorApprenticeshipAustralianStateCode);
                    else if (!Enum.IsDefined(typeof(StateCode), priorApprenticeship.StateCode.ToUpper()))
                        exceptionBuilder.AddException(ValidationExceptionType.InvalidPriorApprenticeshipAustralianStateCode);
                }
            }
            else
                exceptionBuilder.AddException(ValidationExceptionType.InvalidPriorApprenticeshipCountryCode);

            return exceptionBuilder;
        }

        public async Task<ValidationExceptionBuilder> ValidatePriorQualificationsAsync(IQualificationAttributes qualification)
        {
            var exceptionBuilder = new ValidationExceptionBuilder();
            var tasks = new List<Task>();
            if (!qualification.QualificationLevel.IsNullOrEmpty())
            {
                tasks.Add(ValidateCodeAsync(exceptionBuilder, CodeTypes.QualificationLevel, qualification.QualificationLevel, ValidationExceptionType.InvalidQualificationLevel));
            }
            if (!qualification.QualificationANZSCOCode.IsNullOrEmpty())
            {
                tasks.Add(ValidateCodeAsync(exceptionBuilder, CodeTypes.ANZSCOCode, qualification.QualificationANZSCOCode, ValidationExceptionType.InvalidQualificationANZSCO));
            }
            await tasks.WaitAndThrowAnyExceptionFound();
            return exceptionBuilder;
        }
    }
}