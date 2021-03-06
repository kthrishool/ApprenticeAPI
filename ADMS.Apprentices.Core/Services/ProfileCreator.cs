using System;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared;
using Adms.Shared.Extensions;

namespace ADMS.Apprentices.Core.Services
{
    public class ProfileCreator : IProfileCreator
    {
        private readonly IRepository repository;
        private readonly IProfileValidator profileValidator;
        private readonly IUSIVerify usiVerify;
        private const string defaultCountryCode = "61";
        public ProfileCreator(IRepository repository,
            IProfileValidator profileValidator,
            IUSIVerify usiVerify)
        {
            this.repository = repository;
            this.profileValidator = profileValidator;
            this.usiVerify = usiVerify;
        }

        public async Task<Profile> CreateAsync(ProfileMessage message)
        {
            var profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                OtherNames = message.OtherNames.Sanitise(),
                PreferredName = message.PreferredName.Sanitise(),
                BirthDate = message.BirthDate.Value,
                EmailAddress = message.EmailAddress.Sanitise(),
                IndigenousStatusCode = message.IndigenousStatusCode.Sanitise(),
                SelfAssessedDisabilityCode = message.SelfAssessedDisabilityCode.SanitiseUpper(),
                InterpretorRequiredFlag = message.InterpretorRequiredFlag,
                CitizenshipCode = message.CitizenshipCode.SanitiseUpper(),
                ProfileTypeCode = message.ProfileType.SanitiseUpper(),
                CountryOfBirthCode = message.CountryOfBirthCode.SanitiseUpper(),
                PreferredContactTypeCode = message.PreferredContactTypeCode.SanitiseUpper(),
                LanguageCode = message.LanguageCode.SanitiseUpper(),
                HighestSchoolLevelCode = message.HighestSchoolLevelCode.Sanitise(),
                LeftSchoolDate = message.LeftSchoolDate,
                VisaNumber = message.VisaNumber.Sanitise(),
                NotProvidingUSIReasonCode = message.NotProvidingUSIReasonCode.SanitiseUpper()
            };
            if (!message.Phone1.IsNullOrEmpty())
            {                
                profile.Phones.Add(new Phone
                {
                    PhoneNumber = message.Phone1.SanitiseForPhoneNumber(),
                    InternationalPrefix = message.Phone1InternationalPrefix.SanitiseForPhoneNumber().IsNullOrEmpty() ? defaultCountryCode : message.Phone1InternationalPrefix.SanitiseForPhoneNumber(),
                    PhoneTypeCode = PhoneType.PHONE1.ToString()                    
                });                
            }
            if (!message.Phone2.IsNullOrEmpty())
            {
                profile.Phones.Add(new Phone
                {
                    PhoneNumber = message.Phone2.SanitiseForPhoneNumber(),
                    InternationalPrefix = message.Phone2InternationalPrefix.SanitiseForPhoneNumber().IsNullOrEmpty() ? defaultCountryCode : message.Phone2InternationalPrefix.SanitiseForPhoneNumber(),
                    PhoneTypeCode = PhoneType.PHONE2.ToString()
                });
            }

            if (message.GenderCode != null)
            {
                profile.GenderCode = message.GenderCode.SanitiseUpper();
            }

            if (message.ResidentialAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = message.ResidentialAddress.SingleLineAddress.Sanitise(),
                    StreetAddress1 = message.ResidentialAddress.StreetAddress1.Sanitise(),
                    StreetAddress2 = message.ResidentialAddress.StreetAddress2.Sanitise(),
                    StreetAddress3 = message.ResidentialAddress.StreetAddress3.Sanitise(),
                    Locality = message.ResidentialAddress.Locality.Sanitise(),
                    StateCode = message.ResidentialAddress.StateCode.Sanitise(),
                    Postcode = message.ResidentialAddress.Postcode.Sanitise(),
                    AddressTypeCode = AddressType.RESD.ToString(),
                });
            }
            if (message.PostalAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = message.PostalAddress.SingleLineAddress.Sanitise(),
                    StreetAddress1 = message.PostalAddress.StreetAddress1.Sanitise(),
                    StreetAddress2 = message.PostalAddress.StreetAddress2.Sanitise(),
                    StreetAddress3 = message.PostalAddress.StreetAddress3.Sanitise(),
                    Locality = message.PostalAddress.Locality.Sanitise(),
                    StateCode = message.PostalAddress.StateCode.Sanitise(),
                    Postcode = message.PostalAddress.Postcode.Sanitise(),
                    AddressTypeCode = AddressType.POST.ToString(),
                });
            }
            if (!message.USI.IsNullOrEmpty())
            {
                profile.USIs.Add(new ApprenticeUSI
                {
                    USI = message.USI,
                    ActiveFlag = true
                });
            }

            var exceptionBuilder = await profileValidator.ValidateAsync(profile);
            exceptionBuilder.ThrowAnyExceptions();
            
            if (!message.USI.IsNullOrEmpty()) usiVerify.Verify(profile);

            repository.Insert(profile);

            return profile;
        }
    }
}