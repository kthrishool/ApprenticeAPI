using System;
using System.Collections.Generic;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Models
{
    #region WhenInstantiatingAProfileListModel

    [TestClass]
    public class WhenInstantiatingAProfileModel : GivenWhenThen
    {
        private ProfileModel model;
        private Profile profile;
        private Address RestAddress;
        private Address postal;

        protected override void Given()
        {
            ICollection<Address> add = new List<Address>();

            RestAddress = new Address()
            {
                StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
                StreetAddress2 = ProfileConstants.ResidentialAddress.StreetAddress2,
                StreetAddress3 = ProfileConstants.ResidentialAddress.StreetAddress3,
                Locality = ProfileConstants.ResidentialAddress.Locality,
                Postcode = ProfileConstants.ResidentialAddress.Postcode,
                StateCode = ProfileConstants.ResidentialAddress.StateCode,
                SingleLineAddress = ProfileConstants.ResidentialAddress.SingleLineAddress,
                AddressTypeCode = AddressType.RESD.ToString()
            };
            postal = new Address()
            {
                StreetAddress1 = ProfileConstants.PostalAddress.StreetAddress1,
                StreetAddress2 = ProfileConstants.PostalAddress.StreetAddress2,
                StreetAddress3 = ProfileConstants.PostalAddress.StreetAddress3,
                Locality = ProfileConstants.PostalAddress.Locality,
                Postcode = ProfileConstants.PostalAddress.Postcode,
                StateCode = ProfileConstants.PostalAddress.StateCode,
                SingleLineAddress = ProfileConstants.PostalAddress.SingleLineAddress,
                AddressTypeCode = AddressType.POST.ToString()
            };
            add.Add(RestAddress);
            add.Add(postal);

            ICollection<Qualification> qualifications = new List<Qualification>();
            qualifications.Add(ProfileConstants.Qualification);

            profile = new Profile
            {
                Id = ProfileConstants.Id,
                FirstName = ProfileConstants.Surname,
                Surname = ProfileConstants.Firstname,
                OtherNames = ProfileConstants.Secondname,
                PreferredName = ProfileConstants.PreferredName,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress,
                ProfileTypeCode = ProfileConstants.Profiletype,
                CitizenshipCode = ProfileConstants.CitizenshipCode,
                SelfAssessedDisabilityCode = ProfileConstants.SelfAssessedDisabilityCode,
                IndigenousStatusCode = ProfileConstants.IndigenousStatusCode,
                GenderCode = ProfileConstants.GenderCode,
                InterpretorRequiredFlag = ProfileConstants.InterpretorRequiredFlag,
                HighestSchoolLevelCode = ProfileConstants.HighestSchoolLevelCode,
                LeftSchoolMonthCode = ProfileConstants.LeftSchoolMonthCode,
                LeftSchoolYear = ProfileConstants.LeftSchoolYear,
                LeftSchoolDate = new DateTime(ProfileConstants.LeftSchoolYear.Value, 01, 01),
                DeceasedFlag = ProfileConstants.DeceasedFlag,
                ActiveFlag = ProfileConstants.ActiveFlag,
                Addresses = add,
                CountryOfBirthCode = ProfileConstants.CountryOfBirthCode,
                CreatedOn = ProfileConstants.Createdon,
                CreatedBy = ProfileConstants.Createdby,
                UpdatedOn = ProfileConstants.Updatedon,
                UpdatedBy = ProfileConstants.Updatedby,
                Phones = ProfileConstants.PhoneNumbers.Select(c => new Phone() {PhoneNumber = c.PhoneNumber, PreferredPhoneFlag = c.PreferredPhoneFlag}).ToList(),
                LanguageCode = ProfileConstants.LanguageCode,
                PreferredContactType = ProfileConstants.PreferredContactType.ToString(),
                VisaNumber = ProfileConstants.VisaNumber,
                Qualifications = qualifications,
                USIs = new List<ApprenticeUSI>() {new ApprenticeUSI() {USI = ProfileConstants.USI, ActiveFlag = true, USIStatus = "test"}},
                CustomerReferenceNumber = ProfileConstants.CustomerReferenceNumber
            };
        }

        protected override void When()
        {
            model = new ProfileModel(profile);
        }

        [TestMethod]
        public void ReturnsAModel()
        {
            model.Should().NotBeNull();
        }

        /// <summary>
        /// Sets all the properties for the models.
        /// </summary>
        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            model.Id.Should().Be(ProfileConstants.Id);
            model.FirstName.Should().Be(ProfileConstants.Surname);
            model.Surname.Should().Be(ProfileConstants.Firstname);
            model.OtherNames.Should().Be(ProfileConstants.Secondname);
            model.BirthDate.Should().BeCloseTo(ProfileConstants.Birthdate);
            model.EmailAddress.Should().Be(ProfileConstants.Emailaddress);
            model.CreatedOn.Should().BeCloseTo(ProfileConstants.Createdon);
            model.UpdatedOn.Should().BeCloseTo(ProfileConstants.Updatedon);
            model.CreatedBy.Should().Be(ProfileConstants.Createdby);
            model.UpdatedBy.Should().Be(ProfileConstants.Updatedby);
            model.PostalAddress.StreetAddress1.Should().Be(postal.StreetAddress1);
            model.ResidentialAddress.StreetAddress1.Should().Be(RestAddress.StreetAddress1);
            model.ActiveFlag.Should().BeTrue();
            model.DeceasedFlag.Should().BeFalse();
            model.CountryOfBirthCode.Should().Be(ProfileConstants.CountryOfBirthCode);
            model.GenderCode.Should().Be(ProfileConstants.GenderCode);
            model.IndigenousStatusCode.Should().Be(ProfileConstants.IndigenousStatusCode);
            model.InterpretorRequiredFlag.Should().Be(ProfileConstants.InterpretorRequiredFlag);
            model.PhoneNumbers[0].PhoneNumber.Should().Be(ProfileConstants.PhoneNumbers[0].PhoneNumber);
            model.PreferredName.Should().Be(ProfileConstants.PreferredName);
            model.SelfAssessedDisabilityCode.Should().Be(ProfileConstants.SelfAssessedDisabilityCode);
            model.LanguageCode.Should().Be(ProfileConstants.LanguageCode);
            model.PreferredContactType.Should().Be(ProfileConstants.PreferredContactType.ToString());
            model.HighestSchoolLevelCode.Should().Be(ProfileConstants.HighestSchoolLevelCode);
            model.LeftSchoolMonthCode.Should().Be(ProfileConstants.LeftSchoolMonthCode);
            model.LeftSchoolYear.Should().Be(ProfileConstants.LeftSchoolYear);
            model.VisaNumber.Should().Be(ProfileConstants.VisaNumber);
            model.USIVerificationResult.USI.Should().Be(ProfileConstants.USI);
            model.USIVerificationResult.USIStatus.Should().Be("test");
            model.CRNViewFlag.Should().Be(ProfileConstants.CustomerReferenceNumber != null);
        }

        [TestMethod]
        public void SetAddressToNullIfNoAddress()
        {
            profile.Addresses = null;
            model = new ProfileModel(profile);
            model.PostalAddress.Should().BeNull();
            model.ResidentialAddress.Should().BeNull();
        }

        [TestMethod]
        public void SetPhonesToNullIfNoPhones()
        {
            profile.Phones = null;
            model = new ProfileModel(profile);
            model.PhoneNumbers.Should().BeNull();
        }

        [TestMethod]
        public void SetQualificationsToNullIfNoQuals()
        {
            profile.Qualifications = null;
            model = new ProfileModel(profile);
            model.Qualifications.Should().BeNull();
        }
    }

    #endregion
}