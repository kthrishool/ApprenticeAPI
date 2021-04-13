using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared;
using Adms.Shared.Extensions;

namespace ADMS.Apprentice.Core.Services
{
    public class ProfileCreator : IProfileCreator
    {
        private readonly IRepository repository;
        private readonly IProfileValidator profileValidator;

        public ProfileCreator(IRepository repository,
            IProfileValidator profileValidator)
        {
            this.repository = repository;
            this.profileValidator = profileValidator;
        }

        public async Task<Profile> CreateAsync(ProfileMessage message)
        {
            string Sanitise(string s) => s.IsNullOrEmpty() ? null : s.Trim();
            string SanitiseUpper(string s) => s.IsNullOrEmpty() ? null : s.Trim().ToUpper();
            var profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                OtherNames = Sanitise(message.OtherNames),
                PreferredName = Sanitise(message.PreferredName),
                BirthDate = message.BirthDate,
                EmailAddress = Sanitise(message.EmailAddress),
                IndigenousStatusCode = Sanitise(message.IndigenousStatusCode),
                SelfAssessedDisabilityCode = SanitiseUpper(message.SelfAssessedDisabilityCode),
                InterpretorRequiredFlag = message.InterpretorRequiredFlag,
                CitizenshipCode = SanitiseUpper(message.CitizenshipCode),
                ProfileTypeCode =
                    Enum.IsDefined(typeof(ProfileType), message?.ProfileType) ? message.ProfileType : null,
                Phones = message?.PhoneNumbers?.Select(c => new Phone()
                    {PhoneNumber = c, PhoneTypeCode = PhoneType.LandLine.ToString()}).ToList(),
                CountryOfBirthCode = SanitiseUpper(message.CountryOfBirthCode),
                PreferredContactType = message.PreferredContactType,
                
                LanguageCode = SanitiseUpper(message.LanguageCode),
                HighestSchoolLevelCode = Sanitise(message.HighestSchoolLevelCode),  
                LeftSchoolMonthCode = SanitiseUpper(message.LeftSchoolMonthCode),
                LeftSchoolYearCode = Sanitise(message.LeftSchoolYearCode),
            };
            if (message?.GenderCode != null)
            {
                profile.GenderCode = Enum.IsDefined(typeof(GenderType), message?.GenderCode.ToUpper()) ? message.GenderCode.ToUpper() : null;
            }

            if (message.ResidentialAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = Sanitise(message.ResidentialAddress.SingleLineAddress),
                    StreetAddress1 = Sanitise(message.ResidentialAddress.StreetAddress1),
                    StreetAddress2 = Sanitise(message.ResidentialAddress.StreetAddress2),
                    StreetAddress3 = Sanitise(message.ResidentialAddress.StreetAddress3),
                    Locality = Sanitise(message.ResidentialAddress.Locality),
                    StateCode = Sanitise(message.ResidentialAddress.StateCode),
                    Postcode = Sanitise(message.ResidentialAddress.Postcode),
                    AddressTypeCode = AddressType.RESD.ToString(),
                });
            }
            if (message.PostalAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = Sanitise(message.PostalAddress.SingleLineAddress),
                    StreetAddress1 = Sanitise(message.PostalAddress.StreetAddress1),
                    StreetAddress2 = Sanitise(message.PostalAddress.StreetAddress2),
                    StreetAddress3 = Sanitise(message.PostalAddress.StreetAddress3),
                    Locality = Sanitise(message.PostalAddress.Locality),
                    StateCode = Sanitise(message.PostalAddress.StateCode),
                    Postcode = Sanitise(message.PostalAddress.Postcode),
                    AddressTypeCode = AddressType.POST.ToString(),
                });
            }

            await profileValidator.ValidateAsync(profile);
            repository.Insert(profile);

            return profile;
        }
    }
}