using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using Adms.Shared;
using Adms.Shared.Exceptions;
using Adms.Shared.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ADMS.Apprentice.Core.Services.Validators
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


        private async Task ValidateCode(ValidationExceptionBuilder exceptionBuilder, String CodeName, string codevalue, ValidationExceptionType exception)
        {
            IList<ListCodeResponseV1> validCodes = await referenceDataClient.GetListCodes(CodeName, codevalue, true, true);
            if (!validCodes.Any())
            {
                exceptionBuilder.AddException(exception);
            }
        }
         
        private void ValidatePreferredContactType(ValidationExceptionBuilder exceptionBuilder, Profile profile)
        {
            // if profileType is Mobile we need atleast one mobile phone.
            switch (profile.PreferredContactType)
            {
                case nameof(PreferredContactType.MOBILE) or nameof(PreferredContactType.SMS):
                    if (profile.Phones == null || profile.Phones.Any(c => c.PhoneNumber.StartsWith("04")) == false)
                    {
                        exceptionBuilder.AddException(ValidationExceptionType.MobilePreferredContactIsInvalid);
                    }
                    break;
                case nameof(PreferredContactType.PHONE):
                    if (profile.Phones == null || profile.Phones.Any() == false)
                    {
                        exceptionBuilder.AddException(ValidationExceptionType.PhonePreferredContactisInvalid);
                    }
                    break;
                case nameof(PreferredContactType.EMAIL):
                    if (string.IsNullOrEmpty(profile.EmailAddress))
                    {
                        exceptionBuilder.AddException(ValidationExceptionType.EmailPreferredContactisInvalid);
                    }
                    break;
                case nameof(PreferredContactType.MAIL):
                    if (profile.Addresses == null || profile.Addresses.Any() == false)
                    {
                        exceptionBuilder.AddException(ValidationExceptionType.MailPreferredContactisInvalid);
                    }
                    break;
                default:
                    exceptionBuilder.AddException(ValidationExceptionType.InvalidPreferredContactCode);
                    break;
            }
            // validate rules based on the type of contact
        }

        public async Task<ValidationExceptionBuilder> ValidateAsync(Profile profile)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            var tasks = new List<Task>();

            if (!string.IsNullOrEmpty(profile.CountryOfBirthCode))
            {
                tasks.Add(ValidateCode(exceptionBuilder, CodeTypes.country, profile.CountryOfBirthCode, ValidationExceptionType.InvalidCountryCode));
            }
            if (!string.IsNullOrEmpty(profile.LanguageCode))
            {
                tasks.Add(ValidateCode(exceptionBuilder, CodeTypes.language, profile.LanguageCode, ValidationExceptionType.InvalidLanguageCode));
            }
            if (!string.IsNullOrEmpty(profile.HighestSchoolLevelCode))
            {
                tasks.Add(ValidateCode(exceptionBuilder, CodeTypes.schoolLevel, profile.HighestSchoolLevelCode, ValidationExceptionType.InvalidHighestSchoolLevelCode));
            }
            if (!string.IsNullOrEmpty(profile.PreferredContactType))
            {
                 ValidatePreferredContactType(exceptionBuilder, profile);
            }            
            await tasks.WaitAndThrowAnyExceptionFound(); 
            return exceptionBuilder;
        }

        public async Task<ValidationExceptionBuilder> ValidateAsync(Qualification qualification)
        {
            var exceptionBuilder = new ValidationExceptionBuilder(exceptionFactory);
            var tasks = new List<Task>();
            if (!qualification.QualificationLevel.IsNullOrEmpty())
            {
                tasks.Add(ValidateCode(exceptionBuilder, CodeTypes.qualificationLevel, qualification.QualificationLevel, ValidationExceptionType.InvalidQualificationLevel));
            }
            if (!qualification.QualificationANZSCOCode.IsNullOrEmpty())
            {
                tasks.Add(ValidateCode(exceptionBuilder, CodeTypes.ANZSCOCode, qualification.QualificationANZSCOCode, ValidationExceptionType.InvalidQualificationANZSCO));
            }
            await tasks.WaitAndThrowAnyExceptionFound();
            return exceptionBuilder;
        }
    }
}