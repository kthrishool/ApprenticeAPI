using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;

namespace ADMS.Apprentice.Core.Models
{
    public record ProfileListModel
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
        public string CitizenshipCode { get; }
        public string EducationLevelCode { get; }
        public string LeftSchoolMonthCode { get; }
        public string LeftSchoolYearCode { get; }
        public string ProfileTypeCode { get; }
        public bool DeceasedFlag { get; }
        public bool ActiveFlag { get; }
        public List<string> Phones { get; set; }

        public ProfileAddressMessage ResidentialAddress { get; set; }
        public ProfileAddressMessage PostalAddress { get; set; }

        public DateTime? CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime? UpdatedOn { get; }
        public string UpdatedBy { get; }

        public ProfileListModel(Profile apprentice)
        {
            Id = apprentice.Id;
            Surname = apprentice.Surname;
            FirstName = apprentice.FirstName;
            OtherNames = apprentice.OtherNames;
            PreferredName = apprentice.PreferredName;
            BirthDate = apprentice.BirthDate;
            GenderCode = apprentice.GenderCode;
            EmailAddress = apprentice.EmailAddress;
            SelfAssessedDisabilityCode = apprentice.SelfAssessedDisabilityCode;
            IndigenousStatusCode = apprentice.IndigenousStatusCode;
            CitizenshipCode = apprentice.CitizenshipCode;
            EducationLevelCode = apprentice.EducationLevelCode;
            LeftSchoolMonthCode = apprentice.LeftSchoolMonthCode;
            LeftSchoolYearCode = apprentice.LeftSchoolYearCode;
            ProfileTypeCode = apprentice?.ProfileTypeCode?.ToString();
            DeceasedFlag = apprentice.DeceasedFlag;
            ActiveFlag = apprentice.ActiveFlag;
            CreatedOn = apprentice.CreatedOn;
            CreatedBy = apprentice.CreatedBy;
            UpdatedOn = apprentice.UpdatedOn;
            UpdatedBy = apprentice.UpdatedBy;
            if (apprentice?.Phones?.Count() > 0)
                Phones = apprentice.Phones.Select(c => c.PhoneNumber).ToList();
            if (apprentice.Addresses.Any(c => c.AddressTypeCode == AddressType.RESD.ToString()))
                ResidentialAddress = apprentice.Addresses.Where(c => c.AddressTypeCode == AddressType.RESD.ToString()).Select(c => new ProfileAddressMessage
                {
                    Postcode = c.Postcode,
                    StateCode = c.StateCode,
                    SingleLineAddress = c.SingleLineAddress,
                    Locality = c.Locality,
                    StreetAddress1 = c.StreetAddress1,
                    StreetAddress2 = c.StreetAddress2,
                    StreetAddress3 = c.StreetAddress3
                }).SingleOrDefault();
            if (apprentice.Addresses.Any(c => c.AddressTypeCode == AddressType.POST.ToString()))
                PostalAddress = apprentice.Addresses.Where(c => c.AddressTypeCode == AddressType.POST.ToString()).Select(c => new ProfileAddressMessage
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
    }
}