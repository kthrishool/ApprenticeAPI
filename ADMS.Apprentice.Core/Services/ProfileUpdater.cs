﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Helpers;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services.Validators;
using Adms.Shared.Attributes;
using Adms.Shared.Extensions;
using System.Collections.Generic;

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

            UpdatePhone(profile, message.PhoneNumbers);            
           
            //if no addresses specified clear all the addresses.
            if (message.ResidentialAddress == null && message.PostalAddress == null)
                profile.Addresses.Clear();

            if (message.ResidentialAddress != null)
            {   
                //update the existing residential address if exist or add new address
                UpdateAddress(profile, message.ResidentialAddress, AddressType.RESD.ToString());                               
            }
            if (message.PostalAddress != null)
            {
                //update the existing postal address if exist or add new address                
                UpdateAddress(profile, message.PostalAddress, AddressType.POST.ToString());
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
            UpdateUSI(profile, message.USI);
            
            await profileValidator.ValidateAsync(profile);

            return profile;
        }

        public void UpdateAddress(Profile profile, ProfileAddressMessage addressMessage, string addressTypeCode)
        {
            if (profile.Addresses.Count > 0 && profile.Addresses.Any(x => x.AddressTypeCode == addressTypeCode))
            {
                var updatedAddress = profile.Addresses.Where(x => x.AddressTypeCode == addressTypeCode).SingleOrDefault();

                updatedAddress.SingleLineAddress = addressMessage.SingleLineAddress.Sanitise();
                updatedAddress.StreetAddress1 = addressMessage.StreetAddress1.Sanitise();
                updatedAddress.StreetAddress2 = addressMessage.StreetAddress2.Sanitise();
                updatedAddress.StreetAddress3 = addressMessage.StreetAddress3.Sanitise();
                updatedAddress.Locality = addressMessage.Locality.Sanitise();
                updatedAddress.StateCode = addressMessage.StateCode.Sanitise();
                updatedAddress.Postcode = addressMessage.Postcode.Sanitise();
                updatedAddress.AddressTypeCode = addressTypeCode;
            }
            else
            {
                profile.Addresses.Add(new Address()
                {
                    SingleLineAddress = addressMessage.SingleLineAddress.Sanitise(),
                    StreetAddress1 = addressMessage.StreetAddress1.Sanitise(),
                    StreetAddress2 = addressMessage.StreetAddress2.Sanitise(),
                    StreetAddress3 = addressMessage.StreetAddress3.Sanitise(),
                    Locality = addressMessage.Locality.Sanitise(),
                    StateCode = addressMessage.StateCode.Sanitise(),
                    Postcode = addressMessage.Postcode.Sanitise(),
                    AddressTypeCode = addressTypeCode,
                });
            }
        }

        public void UpdateUSI(Profile profile, string usi)
        {
            var currentUSI = profile.USIs.Where(x => x.ActiveFlag == true).SingleOrDefault();
            if (currentUSI == null && !usi.IsNullOrEmpty())
            {
                //add the new USI
                profile.USIs.Add(new ApprenticeUSI { USI = usi, ActiveFlag = true });
            }
            else if (currentUSI != null && !usi.IsNullOrEmpty() && currentUSI.USI != usi)
            {
                //set the activeFlag to false of current active USI and add the new USI                
                currentUSI.ActiveFlag = false;
                profile.USIs.Add(new ApprenticeUSI { USI = usi, ActiveFlag = true, USIChangeReason = $"Updating USI from { currentUSI.USI } to { usi }" });
            }
            else if (currentUSI != null && usi.IsNullOrEmpty())
            {
                //set the activeFlag to false of current active USI               
                currentUSI.ActiveFlag = false;
            }
        }

        public void UpdatePhone(Profile profile, List<UpdatePhoneNumberMessage> phoneMessage)
        {
            if (phoneMessage == null || phoneMessage.Count == 0)
            {
                //if no phones message passed, remove existing phones
                profile.Phones.Clear();
            }
            else if (profile.Phones.Count == 0)
            {
                //if no phone details for the profile , add the new ones
                profile.Phones = phoneMessage.Select(c => new Phone()
                { PhoneNumber = c.PhoneNumber, PreferredPhoneFlag = c.PreferredPhoneFlag }).ToList();
            }
            else
            {
                //update the existing ones, add new ones and remove the ones not in message                
                
                var profilePhoneIds = profile.Phones.Select(x => x.Id).ToList();
                var messagePhoneIds = phoneMessage.Select(x => x.Id).ToList();
               
                var toAdd = phoneMessage.Where(x => x.Id == 0).ToList();
                var toRemove = profile.Phones.Where(x => !messagePhoneIds.Contains(x.Id)).ToList();
                var toUpdate = phoneMessage.Where(x => profilePhoneIds.Contains(x.Id)).ToList();

                toAdd.ForEach(x => profile.Phones.Add(new Phone() { PhoneNumber = x.PhoneNumber, PreferredPhoneFlag = x.PreferredPhoneFlag }));
                toRemove.ForEach(x => profile.Phones.Remove(x));
                toUpdate.ForEach(x =>
                {
                    profile.Phones.Where(y => y.Id == x.Id).SingleOrDefault().PhoneNumber = x.PhoneNumber;
                    profile.Phones.Where(y => y.Id == x.Id).SingleOrDefault().PreferredPhoneFlag = x.PreferredPhoneFlag;
                });                   
            }
        }


        public void UpdateCRN(Profile profile, ServiceAustraliaUpdateMessage message)
        {
            profile.CustomerReferenceNumber = message.CustomerReferenceNumber.Sanitise();
             
        }
    }
}