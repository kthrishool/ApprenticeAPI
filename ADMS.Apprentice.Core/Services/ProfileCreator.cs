using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services.Validators;
using Adms.Shared;

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
            var profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                OtherNames = message.OtherNames.Sanitise(),
                PreferredName = message.PreferredName.Sanitise(),
                BirthDate = message.BirthDate,
                EmailAddress = message.EmailAddress.Sanitise(),
                IndigenousStatusCode = message.IndigenousStatusCode.Sanitise(),
                SelfAssessedDisabilityCode = message.SelfAssessedDisabilityCode.SanitiseUpper(),
                InterpretorRequiredFlag = message.InterpretorRequiredFlag,
                CitizenshipCode = message.CitizenshipCode.SanitiseUpper(),
                ProfileTypeCode =
                    Enum.IsDefined(typeof(ProfileType), message?.ProfileType) ? message.ProfileType : null,
                Phones = message.PhoneNumbers?.Select(c => new Phone()
                    {PhoneNumber = c.PhoneNumber, PreferredPhoneFlag = c.PreferredPhoneFlag}).ToList(),
                CountryOfBirthCode = message.CountryOfBirthCode.SanitiseUpper(),
                PreferredContactType = message.PreferredContactType.SanitiseUpper(),

                LanguageCode = message.LanguageCode.SanitiseUpper(),
                HighestSchoolLevelCode = message.HighestSchoolLevelCode.Sanitise(),
                LeftSchoolMonthCode = message.LeftSchoolMonthCode.SanitiseUpper(),
                LeftSchoolYearCode = message.LeftSchoolYearCode.SanitiseUpper(),
                Qualifications = message.Qualifications?.Select(q => new Qualification()
                {
                    QualificationCode = q.QualificationCode.Sanitise(),
                    QualificationDescription = q.QualificationDescription.Sanitise(),
                    QualificationLevel = q.QualificationLevel.Sanitise(),
                    QualificationANZSCOCode = q.QualificationANZSCOCode.Sanitise(),
                    StartMonth = q.StartMonth.SanitiseUpper(),
                    StartYear = q.StartYear,
                    EndMonth = q.EndMonth.SanitiseUpper(),
                    EndYear = q.EndYear,
                }).ToList(),
            };
           
            if (message.GenderCode != null)
            {
                profile.GenderCode = Enum.IsDefined(typeof(GenderType), message.GenderCode.ToUpper()) ? message.GenderCode.ToUpper() : null;
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

            await profileValidator.ValidateAsync(profile);
            repository.Insert(profile);

            return profile;
        }
    }
}