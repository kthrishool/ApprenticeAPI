using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentice.Core.Entities;

namespace ADMS.Apprentice.Core.Models
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
        public string LeftSchoolMonthCode { get; }
        public string LeftSchoolYearCode { get; }
        public string ProfileTypeCode { get; }
        public bool DeceasedFlag { get; }
        public bool ActiveFlag { get; }
        public string CountryOfBirthCode { get; }
        public string LanguageCode { get; }
        public string HighestSchoolLevelCode { get; }
        public List<PhoneNumberModel> Phones { get; set; }
        public string PreferredContactCode { get; set; }
        public ProfileAddressModel ResidentialAddress { get; set; }
        public ProfileAddressModel PostalAddress { get; set; }
        public List<ProfileQualificationModel> Qualifications { get; set; }

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

            EmailAddress = apprentice.EmailAddress;
            SelfAssessedDisabilityCode = apprentice.SelfAssessedDisabilityCode;
            IndigenousStatusCode = apprentice.IndigenousStatusCode;
            CitizenshipCode = apprentice.CitizenshipCode;
            InterpretorRequiredFlag = apprentice.InterpretorRequiredFlag;
            LeftSchoolMonthCode = apprentice.LeftSchoolMonthCode;
            LeftSchoolYearCode = apprentice.LeftSchoolYearCode;
            ProfileTypeCode = apprentice.ProfileTypeCode;
            GenderCode = apprentice.GenderCode;
            CountryOfBirthCode = apprentice.CountryOfBirthCode;
            LanguageCode = apprentice.LanguageCode;
            HighestSchoolLevelCode = apprentice.HighestSchoolLevelCode;
            PreferredContactCode = apprentice.PreferredContactType;
            DeceasedFlag = apprentice.DeceasedFlag;
            ActiveFlag = apprentice.ActiveFlag;
            CreatedOn = apprentice.CreatedOn;
            CreatedBy = apprentice.CreatedBy;
            UpdatedOn = apprentice.UpdatedOn;
            UpdatedBy = apprentice.UpdatedBy;
            Version = apprentice.Version;
            if (apprentice?.Phones?.Count > 0)
                Phones = apprentice.Phones.Select(c => new PhoneNumberModel() {PhoneNumber = c.PhoneNumber, PreferredPhoneFlag = c.PreferredPhoneFlag, PhoneTypeCode = c.PhoneTypeCode}).ToList();
            if (apprentice.Addresses?.Count > 0 && apprentice.Addresses.Any(c => c.AddressTypeCode == AddressType.RESD.ToString()))
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
            if (apprentice.Addresses?.Count > 0 && apprentice.Addresses.Any(c => c.AddressTypeCode == AddressType.POST.ToString()))
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

            Qualifications = apprentice.Qualifications?.Select(q => new ProfileQualificationModel(q)).ToList();
        }
    }
}