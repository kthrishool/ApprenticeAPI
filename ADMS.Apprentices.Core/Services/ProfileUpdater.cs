using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Helpers;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared.Attributes;
using Adms.Shared.Extensions;

namespace ADMS.Apprentices.Core.Services
{
    [RegisterWithIocContainer]
    public class ProfileUpdater : IProfileUpdater
    {
        private readonly IProfileValidator profileValidator;
        private readonly IDeceasedValidator deceasedValidator;
        private readonly IUSIVerify usiVerify;
        bool triggerUsiVerification = false;
        private const string defaultCountryCode = "+61";

        public ProfileUpdater(IProfileValidator profileValidator, IDeceasedValidator deceasedValidator, IUSIVerify usiVerify)
        {
            this.profileValidator = profileValidator;
            this.deceasedValidator = deceasedValidator;
            this.usiVerify = usiVerify;
        }

        public void UpdateDeceasedFlag(Profile profile, bool deceased, DateTime? deceasedDate)
        {
            profile.DeceasedFlag = deceased;
            profile.DeceasedDate = deceasedDate;
            profile.ActiveFlag = !deceased;
            //No need to update the Inactive date if InactiveDate is already set ie scenario when setting deceased flag to true or updating the deceased date of an already inactive record
            profile.InactiveDate = profile.ActiveFlag ? null : profile.InactiveDate.HasValue? profile.InactiveDate : System.DateTime.Now.Date;
            deceasedValidator.Validate(profile);
            //Need to think about ending any TCs that are active and therefore any TSL instalments pending. 
        }

        public void Update(Profile profile, AdminUpdateMessage message)
        {
            if (message.DeceasedFlag.HasValue)
                UpdateDeceasedFlag(profile, message.DeceasedFlag.Value, message.DeceasedDate);
        }

        public async Task<Profile> Update(Profile profile, UpdateProfileMessage message)
        {
            triggerUsiVerification = profile.Surname != message.Surname || profile.FirstName != message.FirstName || profile.BirthDate != message.BirthDate.Value;
            profile.Surname = message.Surname;
            profile.FirstName = message.FirstName;
            profile.OtherNames = message.OtherNames.Sanitise();
            profile.PreferredName = message.PreferredName.Sanitise();
            profile.BirthDate = message.BirthDate.Value;
            profile.GenderCode = message.GenderCode.SanitiseUpper();
            profile.ProfileTypeCode = message.ProfileType.SanitiseUpper();
            profile.EmailAddress = message.EmailAddress.Sanitise();

            UpdatePhone(profile, message.Phone1, message.Phone1CountryCode, PhoneType.PHONE1.ToString());
            UpdatePhone(profile, message.Phone2, message.Phone2CountryCode, PhoneType.PHONE2.ToString());

            //if no addresses specified clear address if exists.
            if (message.ResidentialAddress == null)
                profile.Addresses.Remove(profile.Addresses.SingleOrDefault(x => x.AddressTypeCode == AddressType.RESD.ToString()));

            if (message.PostalAddress == null)
                profile.Addresses.Remove(profile.Addresses.SingleOrDefault(x => x.AddressTypeCode == AddressType.POST.ToString()));

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
            profile.PreferredContactTypeCode = message.PreferredContactTypeCode.SanitiseUpper();

            //school details
            profile.HighestSchoolLevelCode = message.HighestSchoolLevelCode.Sanitise();
            profile.LeftSchoolDate = message.LeftSchoolDate;

            //characteristics
            profile.LanguageCode = message.LanguageCode.SanitiseUpper();
            profile.InterpretorRequiredFlag = message.InterpretorRequiredFlag;
            profile.CountryOfBirthCode = message.CountryOfBirthCode.SanitiseUpper();
            profile.CitizenshipCode = message.CitizenshipCode.SanitiseUpper();
            profile.IndigenousStatusCode = message.IndigenousStatusCode.Sanitise();
            profile.SelfAssessedDisabilityCode = message.SelfAssessedDisabilityCode.SanitiseUpper();
            profile.VisaNumber = message.VisaNumber.Sanitise();
            profile.NotProvidingUSIReasonCode = message.NotProvidingUSIReasonCode.SanitiseUpper();

            //USI
            UpdateUSI(profile, message.USI.Sanitise(), message.USIChangeReason);

            var exceptionBuilder = await profileValidator.ValidateAsync(profile);
            exceptionBuilder.ThrowAnyExceptions();

            //trigger USI verfication, if USI attributes has changed
            if (triggerUsiVerification)
                usiVerify.Verify(profile);

            return profile;
        }

        public void UpdatePhone(Profile profile, string newPhoneNumber, string newCountryCode, string phoneTypeCode)
        {
            var currentPhone = profile.Phones.SingleOrDefault(x => x.PhoneTypeCode == phoneTypeCode);
            
            if (newPhoneNumber.IsNullOrEmpty())  //new phone is null, so remove if exist.
                profile.Phones.Remove(profile.Phones.SingleOrDefault(x => x.PhoneTypeCode == phoneTypeCode));
            else if (currentPhone == null) // newPhone not null and current phone is null, add new phone
            {
                profile.Phones.Add(new Phone()
                {
                    PhoneNumber = newPhoneNumber.Sanitise(),
                    CountryCode = newCountryCode.Sanitise().IsNullOrEmpty() ? defaultCountryCode : newCountryCode.Sanitise(),
                    PhoneTypeCode = phoneTypeCode
                });
            }
            else //current phone is not null and new phone number not null, so update existing.
            {
                currentPhone.PhoneNumber = newPhoneNumber.Sanitise();
                currentPhone.CountryCode = newCountryCode.Sanitise().IsNullOrEmpty() ? defaultCountryCode : newCountryCode.Sanitise();
            }             
        }

        public void UpdateAddress(Profile profile, ProfileAddressMessage addressMessage, string addressTypeCode)
        {
            if (profile.Addresses.Count > 0 && profile.Addresses.Any(x => x.AddressTypeCode == addressTypeCode))
            {
                var updatedAddress = profile.Addresses.SingleOrDefault(x => x.AddressTypeCode == addressTypeCode);

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

        public void UpdateUSI(Profile profile, string usi, string usichangereason)
        {
            var currentUSI = profile.USIs.LastOrDefault(x => x.ActiveFlag == true);
            if (currentUSI == null && !usi.IsNullOrEmpty())
            {
                //add the new USI
                profile.USIs.Add(new ApprenticeUSI {USI = usi, ActiveFlag = true});
                triggerUsiVerification = true;
            }
            else if (currentUSI != null && !usi.IsNullOrEmpty() && currentUSI.USI != usi)
            {
                //set the activeFlag to false of current active USI and add the new USI                
                currentUSI.ActiveFlag = false;
                profile.USIs.Add(new ApprenticeUSI {USI = usi, ActiveFlag = true, USIChangeReason = usichangereason});

                triggerUsiVerification = true;
            }
            else if (currentUSI != null && usi.IsNullOrEmpty() && currentUSI.USI != usi)
            {
                //set the activeFlag to false of current active USI               
                currentUSI.ActiveFlag = false;
                triggerUsiVerification = false;
            }
        }

        //******************NOT IN USE**********************
        //public void UpdatePhone(Profile profile, List<UpdatePhoneNumberMessage> phoneMessage)
        //{
        //    if (phoneMessage == null || phoneMessage.Count == 0)
        //    {
        //        //if no phones message passed, remove existing phones
        //        profile.Phones.Clear();
        //    }
        //    else if (profile.Phones.Count == 0)
        //    {
        //        //if no phone details for the profile , add the new ones
        //        foreach (PhoneNumberMessage phone in phoneMessage)
        //        {
        //            profile.Phones.Add(new Phone
        //            {
        //                PhoneNumber = phone.PhoneNumber,
        //                PhoneTypeCode = phone.PhoneTypeCode,
        //                PreferredPhoneFlag = phone.PreferredPhoneFlag
        //            });
        //        }
        //    }
        //    else
        //    {
        //        //update the existing ones, add new ones and remove the ones not in message                

        //        var profilePhoneIds = profile.Phones.Select(x => x.Id).ToList();
        //        var messagePhoneIds = phoneMessage.Select(x => x.Id).ToList();

        //        var toAdd = phoneMessage.Where(x => x.Id == 0).ToList();
        //        var toRemove = profile.Phones.Where(x => !messagePhoneIds.Contains(x.Id)).ToList();
        //        var toUpdate = phoneMessage.Where(x => profilePhoneIds.Contains(x.Id)).ToList();

        //        toAdd.ForEach(x => profile.Phones.Add(new Phone() { PhoneNumber = x.PhoneNumber, PreferredPhoneFlag = x.PreferredPhoneFlag, PhoneTypeCode = x.PhoneTypeCode }));
        //        toRemove.ForEach(x => profile.Phones.Remove(x));
        //        toUpdate.ForEach(x =>
        //        {
        //            profile.Phones.SingleOrDefault(y => y.Id == x.Id).PhoneNumber = x.PhoneNumber;
        //            profile.Phones.SingleOrDefault(y => y.Id == x.Id).PreferredPhoneFlag = x.PreferredPhoneFlag;
        //            profile.Phones.SingleOrDefault(y => y.Id == x.Id).PhoneTypeCode = x.PhoneTypeCode;
        //        });
        //    }
        //}


        public void UpdateCRN(Profile profile, ServiceAustraliaUpdateMessage message)
        {
            profile.CustomerReferenceNumber = message.CustomerReferenceNumber.Sanitise();
        }
    }
}