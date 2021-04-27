using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.Core.Helpers;
using System;
using System.Linq;
using Adms.Shared.Extensions;

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

        public void UpdateDeceasedFlag(Profile profile, bool deceased)
        {
            profile.DeceasedFlag = deceased;
            //Need to think about ending any TCs that are active and therefore any TSL instalments pending. 
        }

        public void Update(Profile profile, AdminUpdateMessage message)
        {
            profile.DeceasedFlag = message.DeceasedFlag; //or call UpdateDeceasedFlag function??  
        }

        public async Task<Profile> Update(Profile profile, UpdateProfileMessage message)
        {
            
            SetBasicDetails(profile, message);
            SetContactDetails(profile, message);            
            SetSchoolDetails(profile, message);
            SetOtherDetails(profile, message);
            SetQualificationDetails(profile, message);

            await profileValidator.ValidateAsync(profile);

            return profile;
        }

        private static void SetBasicDetails(Profile profile, UpdateProfileMessage message)
        {
            if (message?.BasicDetails == null) return;

            profile.Surname = message.BasicDetails.Surname;
            profile.FirstName = message.BasicDetails.FirstName;
            profile.OtherNames = message.BasicDetails.OtherNames.Sanitise();
            profile.PreferredName = message.BasicDetails.PreferredName.Sanitise();
            profile.BirthDate = message.BasicDetails.BirthDate;
            profile.GenderCode = message.BasicDetails.GenderCode.IsNullOrEmpty()? null :
                Enum.IsDefined(typeof(GenderType), message.BasicDetails.GenderCode.SanitiseUpper()) ? message.BasicDetails.GenderCode.SanitiseUpper() : null;
            profile.ProfileTypeCode = Enum.IsDefined(typeof(ProfileType), message.BasicDetails.ProfileType.SanitiseUpper()) ? message.BasicDetails.ProfileType.SanitiseUpper() : null;
        }

        private static void SetContactDetails(Profile profile, UpdateProfileMessage message)
        {
            if (message?.ContactDetails == null) return;

            profile.EmailAddress = message.ContactDetails.EmailAddress.Sanitise();
            
            //remove existing phones
            profile.Phones.Clear();
            profile.Phones = message.ContactDetails.PhoneNumbers?.Select(c => new Phone()
            { PhoneNumber = c.PhoneNumber, PreferredPhoneFlag = c.PreferredPhoneFlag }).ToList();

            //remove the existing addresses
            profile.Addresses.Clear();
            if (message.ContactDetails.ResidentialAddress != null)
            {                
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = message.ContactDetails.ResidentialAddress.SingleLineAddress.Sanitise(),
                    StreetAddress1 = message.ContactDetails.ResidentialAddress.StreetAddress1.Sanitise(),
                    StreetAddress2 = message.ContactDetails.ResidentialAddress.StreetAddress2.Sanitise(),
                    StreetAddress3 = message.ContactDetails.ResidentialAddress.StreetAddress3.Sanitise(),
                    Locality = message.ContactDetails.ResidentialAddress.Locality.Sanitise(),
                    StateCode = message.ContactDetails.ResidentialAddress.StateCode.Sanitise(),
                    Postcode = message.ContactDetails.ResidentialAddress.Postcode.Sanitise(),
                    AddressTypeCode = AddressType.RESD.ToString(),
                });
            }
            if (message.ContactDetails.PostalAddress != null)
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = message.ContactDetails.PostalAddress.SingleLineAddress.Sanitise(),
                    StreetAddress1 = message.ContactDetails.PostalAddress.StreetAddress1.Sanitise(),
                    StreetAddress2 = message.ContactDetails.PostalAddress.StreetAddress2.Sanitise(),
                    StreetAddress3 = message.ContactDetails.PostalAddress.StreetAddress3.Sanitise(),
                    Locality = message.ContactDetails.PostalAddress.Locality.Sanitise(),
                    StateCode = message.ContactDetails.PostalAddress.StateCode.Sanitise(),
                    Postcode = message.ContactDetails.PostalAddress.Postcode.Sanitise(),
                    AddressTypeCode = AddressType.POST.ToString(),
                });
            }
            profile.PreferredContactType = message.ContactDetails.PreferredContactType.SanitiseUpper();
        }

        private static void SetSchoolDetails(Profile profile, UpdateProfileMessage message)
        {
            if (message?.SchoolDetails == null) return;

            profile.HighestSchoolLevelCode = message.SchoolDetails.HighestSchoolLevelCode.Sanitise();
            profile.LeftSchoolMonthCode = message.SchoolDetails.LeftSchoolMonthCode.SanitiseUpper();
            profile.LeftSchoolYearCode = message.SchoolDetails.LeftSchoolYearCode.SanitiseUpper();
        }

        private static void SetOtherDetails(Profile profile, UpdateProfileMessage message)
        {
            if (message?.OtherDetails == null) return;

            profile.LanguageCode = message.OtherDetails.LanguageCode.SanitiseUpper();
            profile.InterpretorRequiredFlag = message.OtherDetails.InterpretorRequiredFlag;
            profile.CountryOfBirthCode = message.OtherDetails.CountryOfBirthCode.SanitiseUpper();
            profile.CitizenshipCode = message.OtherDetails.CitizenshipCode.SanitiseUpper();

            profile.IndigenousStatusCode = message.OtherDetails.IndigenousStatusCode.Sanitise();
            profile.SelfAssessedDisabilityCode = message.OtherDetails.SelfAssessedDisabilityCode.SanitiseUpper();
            profile.VisaNumber = message.OtherDetails.VisaNumber.Sanitise();
        }
        private static void SetQualificationDetails(Profile profile, UpdateProfileMessage message)
        {
            if (message?.QualificationDetails == null) return;
            
            //clear existing qualifications first.
            profile.Qualifications.Clear();
            profile.Qualifications = message.QualificationDetails.Qualifications.Select(q => new Qualification()
            {
                QualificationCode = q.QualificationCode.Sanitise(),
                QualificationDescription = q.QualificationDescription.Sanitise(),
                QualificationLevel = q.QualificationLevel.Sanitise(),
                QualificationANZSCOCode = q.QualificationANZSCOCode.Sanitise(),
                StartMonth = q.StartMonth.SanitiseUpper(),
                StartYear = q.StartYear,
                EndMonth = q.EndMonth.SanitiseUpper(),
                EndYear = q.EndYear,
            }).ToList();
        }    
    }
}