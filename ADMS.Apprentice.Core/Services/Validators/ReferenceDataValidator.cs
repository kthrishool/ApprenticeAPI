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


        private async Task ValidateCode(String CodeName, string codevalue, ValidationExceptionType exception)
        {
            IList<ListCodeResponseV1> validCodes = await referenceDataClient.GetListCodes(CodeName, codevalue, true, true);
            if (!validCodes.Any())
            {
                throw exceptionFactory.CreateValidationException(exception);
            }
        }

        private async Task ValidatePreferredContactType(Profile profile)
        {
            // if profileType is Mobile we need atleast one mobile phone.
            switch (profile.PreferredContactType)
            {
                case nameof(PreferredContactType.MOBILE) or nameof(PreferredContactType.SMS):

                    if (profile.Phones == null || profile.Phones?.Any(c => c.PhoneNumber.StartsWith("04")) == false)
                    {
                        throw exceptionFactory.CreateValidationException(ValidationExceptionType.MobilePreferredContactIsInvalid);
                    }
                    break;
                case nameof(PreferredContactType.PHONE):
                    if (profile.Phones == null || profile.Phones?.Any() == false)
                    {
                        throw exceptionFactory.CreateValidationException(ValidationExceptionType.PhonePreferredContactisInvalid);
                    }
                    break;
                case nameof(PreferredContactType.EMAIL):
                    if (string.IsNullOrEmpty(profile.EmailAddress))
                    {
                        throw exceptionFactory.CreateValidationException(ValidationExceptionType.EmailPreferredContactisInvalid);
                    }
                    break;
                case nameof(PreferredContactType.MAIL):
                    if (profile.Addresses == null || profile.Addresses?.Any() == false)
                    {
                        throw exceptionFactory.CreateValidationException(ValidationExceptionType.MailPreferredContactisInvalid);
                    }
                    break;
                default:
                    throw exceptionFactory.CreateValidationException(ValidationExceptionType.InvalidPreferredContactCode);
            }


            // validate rules based on the type of contact
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
            if (!string.IsNullOrEmpty(profile.PreferredContactType))
            {
                await ValidatePreferredContactType(profile);
            }               
            
        }
    }
}