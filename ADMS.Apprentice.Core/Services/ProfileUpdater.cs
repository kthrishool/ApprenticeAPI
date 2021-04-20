using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.Core.Helpers;
using System;
using System.Linq;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
    public class ProfileUpdater : IProfileUpdater
    {
        private readonly IProfileValidator profileValidator;
        public ProfileUpdater(IProfileValidator profileValidator)
        {
            this.profileValidator = profileValidator;
        }

        public async Task<Profile> Update(Profile profile, UpdateProfileMessage message)
        {
            profile.Surname = message?.Surname;
            profile.FirstName = message?.FirstName;
            profile.OtherNames = message?.OtherNames.Sanitise();
            profile.PreferredName = message?.PreferredName.Sanitise();
            profile.BirthDate = (System.DateTime)(message?.BirthDate);
            profile.ProfileTypeCode = Enum.IsDefined(typeof(ProfileType), message?.ProfileType) ? message.ProfileType : null;

            profile.EmailAddress = message?.EmailAddress.Sanitise();
            profile.Phones.Clear();
            profile.Phones = message?.PhoneNumbers?.Select(c => new Phone()
            { PhoneNumber = c, PhoneTypeCode = PhoneType.LandLine.ToString() }).ToList();

            //remove the existing addresses
            profile.Addresses.Clear();
            if (message.ResidentialAddress != null)
            {                
                //profile.Addresses.Remove(profile.Addresses.Where(x => x.AddressTypeCode == AddressType.RESD.ToString()).SingleOrDefault());
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
            profile.PreferredContactType = message?.PreferredContactType.SanitiseUpper();

            profile.GenderCode = Enum.IsDefined(typeof(GenderType), message.GenderCode?.ToUpper()) ? message.GenderCode.ToUpper() : null;
            profile.LanguageCode = message?.LanguageCode.SanitiseUpper();
            profile.InterpretorRequiredFlag = message?.InterpretorRequiredFlag;
            profile.CountryOfBirthCode = message?.CountryOfBirthCode.SanitiseUpper();
            profile.CitizenshipCode = message?.CitizenshipCode?.SanitiseUpper();

            profile.IndigenousStatusCode = message?.IndigenousStatusCode.Sanitise();
            profile.SelfAssessedDisabilityCode = message?.SelfAssessedDisabilityCode.SanitiseUpper();
            
            profile.HighestSchoolLevelCode = message.HighestSchoolLevelCode.Sanitise();
            profile.LeftSchoolMonthCode = message.LeftSchoolMonthCode.SanitiseUpper();
            profile.LeftSchoolYearCode = message.LeftSchoolYearCode.SanitiseUpper();

            //clear existing qualifications first.
            profile.Qualifications.Clear();
            profile.Qualifications = message.Qualifications?.Select(q => new Qualification()
            {
                QualificationCode = q.QualificationCode.SanitiseUpper(),
                QualificationDescription = q.QualificationDescription.Sanitise(),
                StartMonth = q.StartMonth.SanitiseUpper(),
                StartYear = q.StartYear.Sanitise(),
                EndMonth = q.EndMonth.SanitiseUpper(),
                EndYear = q.EndYear.Sanitise(),
            }).ToList();

            await profileValidator.ValidateAsync(profile);

            return profile;
        }
    }
}