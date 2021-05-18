using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services.Validators;
using Adms.Shared.Attributes;
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
            profile.Surname = message.Surname;
            profile.FirstName = message.FirstName;
            profile.OtherNames = message.OtherNames.Sanitise();
            profile.PreferredName = message.PreferredName.Sanitise();
            profile.BirthDate = message.BirthDate;
            profile.GenderCode = message.GenderCode.SanitiseUpper();            
            profile.ProfileTypeCode = message.ProfileType.SanitiseUpper();
            profile.EmailAddress = message.EmailAddress.Sanitise();

            //remove existing phones            
            profile.Phones.Clear();
            profile.Phones = message.PhoneNumbers?.Select(c => new Phone()
            { PhoneNumber = c.PhoneNumber, PreferredPhoneFlag = c.PreferredPhoneFlag }).ToList();

            //if no addresses specified clear all the addresses.
            if (message.ResidentialAddress == null && message.PostalAddress == null)
                profile.Addresses.Clear();

            if (message.ResidentialAddress != null)
            {
                //update the existing residential address if exist or add new address
                if (profile.Addresses.Count > 0 && profile.Addresses.Any(x => x.AddressTypeCode == AddressType.RESD.ToString()))
                {
                    var resdAddress = profile.Addresses.Where(x => x.AddressTypeCode == AddressType.RESD.ToString()).SingleOrDefault();

                    resdAddress.SingleLineAddress = message.ResidentialAddress.SingleLineAddress.Sanitise();
                    resdAddress.StreetAddress1 = message.ResidentialAddress.StreetAddress1.Sanitise();
                    resdAddress.StreetAddress2 = message.ResidentialAddress.StreetAddress2.Sanitise();
                    resdAddress.StreetAddress3 = message.ResidentialAddress.StreetAddress3.Sanitise();
                    resdAddress.Locality = message.ResidentialAddress.Locality.Sanitise();
                    resdAddress.StateCode = message.ResidentialAddress.StateCode.Sanitise();
                    resdAddress.Postcode = message.ResidentialAddress.Postcode.Sanitise();
                    resdAddress.AddressTypeCode = AddressType.RESD.ToString();                                     
                }
                else
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
            }
            if (message.PostalAddress != null)
            {
                //update the existing postal address if exist or add new address
                if (profile.Addresses.Count > 0 && profile.Addresses.Any(x => x.AddressTypeCode == AddressType.POST.ToString()))
                {
                    var postAddress = profile.Addresses.Where(x => x.AddressTypeCode == AddressType.POST.ToString()).SingleOrDefault();

                    postAddress.SingleLineAddress = message.PostalAddress.SingleLineAddress.Sanitise();
                    postAddress.StreetAddress1 = message.PostalAddress.StreetAddress1.Sanitise();
                    postAddress.StreetAddress2 = message.PostalAddress.StreetAddress2.Sanitise();
                    postAddress.StreetAddress3 = message.PostalAddress.StreetAddress3.Sanitise();
                    postAddress.Locality = message.PostalAddress.Locality.Sanitise();
                    postAddress.StateCode = message.PostalAddress.StateCode.Sanitise();
                    postAddress.Postcode = message.PostalAddress.Postcode.Sanitise();
                    postAddress.AddressTypeCode = AddressType.POST.ToString();
                }
                else
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
            }
            profile.PreferredContactType = message.PreferredContactType.SanitiseUpper();

            //school details
            profile.HighestSchoolLevelCode = message.HighestSchoolLevelCode.Sanitise();
            profile.LeftSchoolMonthCode = message.LeftSchoolMonthCode.SanitiseUpper();
            profile.LeftSchoolYear = message.LeftSchoolYear;

            //characteristics
            profile.LanguageCode = message.LanguageCode.SanitiseUpper();
            profile.InterpretorRequiredFlag = message.InterpretorRequiredFlag;
            profile.CountryOfBirthCode = message.CountryOfBirthCode.SanitiseUpper();
            profile.CitizenshipCode = message.CitizenshipCode.SanitiseUpper();
            profile.IndigenousStatusCode = message.IndigenousStatusCode.Sanitise();
            profile.SelfAssessedDisabilityCode = message.SelfAssessedDisabilityCode.SanitiseUpper();
            profile.VisaNumber = message.VisaNumber.Sanitise();

            //USI
            var currentUSI = profile.USIs.Where(x => x.ActiveFlag == true).SingleOrDefault();
            if (currentUSI == null && !message.USI.IsNullOrEmpty())
            {
                //add the new USI
                profile.USIs.Add(new ApprenticeUSI { USI = message.USI, ActiveFlag = true });
            }
            else if (currentUSI != null && !message.USI.IsNullOrEmpty() && currentUSI.USI != message.USI)
            {
                //set the activeFlag to false of current active USI and add the new USI                
                currentUSI.ActiveFlag = false;
                profile.USIs.Add(new ApprenticeUSI { USI = message.USI, ActiveFlag = true, USIChangeReason = $"Updating USI from { currentUSI.USI } to { message.USI }" });
            }
            else if (currentUSI != null && message.USI.IsNullOrEmpty())
            {
                //set the activeFlag to false of current active USI               
                currentUSI.ActiveFlag = false;
            }
            
            await profileValidator.ValidateAsync(profile);

            return profile;
        }

        public void UpdateCRN(Profile profile, ServiceAustraliaUpdateMessage message)
        {
            profile.CustomerReferenceNumber = message.CustomerReferenceNumber.Sanitise();
            profileValidator.ValidateCRN(profile);
        }
    }
}