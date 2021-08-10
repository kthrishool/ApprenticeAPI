using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.Core.Models
{
    /// <summary>
    /// Generic apprentice model, containing all information
    /// </summary>
    public record ProfileModel
    {
        public int Id { get; }
        public string Surname { get; }
        public string FirstName { get; }
        public string OtherNames { get; }
        public string PreferredName { get; }
        public string GenderCode { get; }
        public DateTime BirthDate { get; }
        public string EmailAddress { get; }
        public string SelfAssessedDisabilityCode { get; }
        public string IndigenousStatusCode { get; }
        public bool? InterpretorRequiredFlag { get; }
        public string CitizenshipCode { get; }
        public DateTime? LeftSchoolDate { get; }
        public string ProfileType { get; }
        public bool DeceasedFlag { get; }
        public bool ActiveFlag { get; }
        public string CountryOfBirthCode { get; }
        public string LanguageCode { get; }
        public string HighestSchoolLevelCode { get; }
        public string VisaNumber { get; }
        public List<PhoneNumberModel> PhoneNumbers { get; set; }
        public string PreferredContactType { get; set; }
        public ProfileAddressModel ResidentialAddress { get; set; }
        public ProfileAddressModel PostalAddress { get; set; }
        public List<ProfileQualificationModel> Qualifications { get; set; }
        public Boolean CRNViewFlag { get; set; }
        public ProfileUSIModel USIVerificationResult { get; set; }
        public string USI { get; set; }
        public string USIChangeReason { get; set; }
        public string USIExemptionReasonCode { get; set; }
        public DateTime? CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime? UpdatedOn { get; }
        public string UpdatedBy { get; }
        public byte[] Version { get; }

        public ProfileModel(Profile apprentice)
        {
            Id = apprentice.Id;
            Surname = apprentice.Surname;
            FirstName = apprentice.FirstName;
            OtherNames = apprentice.OtherNames;
            PreferredName = apprentice.PreferredName;
            BirthDate = apprentice.BirthDate;

            SelfAssessedDisabilityCode = apprentice.SelfAssessedDisabilityCode;
            IndigenousStatusCode = apprentice.IndigenousStatusCode;
            CitizenshipCode = apprentice.CitizenshipCode;
            InterpretorRequiredFlag = apprentice.InterpretorRequiredFlag;
            LeftSchoolDate = apprentice.LeftSchoolDate;
            ProfileType = apprentice.ProfileTypeCode;
            GenderCode = apprentice.GenderCode;
            CountryOfBirthCode = apprentice.CountryOfBirthCode;
            LanguageCode = apprentice.LanguageCode;
            HighestSchoolLevelCode = apprentice.HighestSchoolLevelCode;
            VisaNumber = apprentice.VisaNumber;
            DeceasedFlag = apprentice.DeceasedFlag;
            CRNViewFlag = apprentice.CustomerReferenceNumber != null;
            ActiveFlag = apprentice.ActiveFlag;
            CreatedOn = apprentice.CreatedOn;
            CreatedBy = apprentice.CreatedBy;
            UpdatedOn = apprentice.UpdatedOn;
            UpdatedBy = apprentice.UpdatedBy;
            Version = apprentice.Version;

            if (!apprentice.DeceasedFlag)
            {
                PreferredContactType = apprentice.PreferredContactType;
                EmailAddress = apprentice.EmailAddress;
                if (apprentice.Phones.Count > 0)
                    PhoneNumbers = apprentice.Phones.Select(c => new PhoneNumberModel() {Id = c.Id, PhoneNumber = c.PhoneNumber, PreferredPhoneFlag = c.PreferredPhoneFlag, PhoneTypeCode = c.PhoneTypeCode}).ToList();
                if (apprentice.Addresses.Count > 0 && apprentice.Addresses.Any(c => c.AddressTypeCode == AddressType.RESD.ToString()))
                    ResidentialAddress = apprentice.Addresses.Where(c => c.AddressTypeCode == AddressType.RESD.ToString()).Select(c => new ProfileAddressModel
                    {
                        Postcode = c.Postcode,
                        StateCode = c.StateCode,
                        SingleLineAddress = c.SingleLineAddress,
                        Locality = c.Locality,
                        StreetAddress1 = c.StreetAddress1,
                        StreetAddress2 = c.StreetAddress2,
                        StreetAddress3 = c.StreetAddress3
                    }).SingleOrDefault();
                if (apprentice.Addresses.Count > 0 && apprentice.Addresses.Any(c => c.AddressTypeCode == AddressType.POST.ToString()))
                    PostalAddress = apprentice.Addresses.Where(c => c.AddressTypeCode == AddressType.POST.ToString()).Select(c => new ProfileAddressModel
                    {
                        Postcode = c.Postcode,
                        StateCode = c.StateCode,
                        SingleLineAddress = c.SingleLineAddress,
                        Locality = c.Locality,
                        StreetAddress1 = c.StreetAddress1,
                        StreetAddress2 = c.StreetAddress2,
                        StreetAddress3 = c.StreetAddress3
                    }).SingleOrDefault();
            }

            Qualifications = apprentice.Qualifications.Select(q => new ProfileQualificationModel(q)).ToList();
            if (apprentice.USIs.Any(c => c.ActiveFlag == true))
            {
                USIVerificationResult = apprentice.USIs.Where(c => c.ActiveFlag == true).Select(c => new ProfileUSIModel(c)).LastOrDefault();
                USI = USIVerificationResult.USI;
                USIChangeReason = USIVerificationResult.USIChangeReason;
            }
            USIExemptionReasonCode = apprentice.USIExemptionReasonCode;
        }
    }
}