using System.Collections.Generic;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Models
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
            ICollection<Address> addresses = new List<Address>();

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
            addresses.Add(RestAddress);
            addresses.Add(postal);

            ICollection<PriorQualification> qualifications = new List<PriorQualification>();
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
                LeftSchoolDate = ProfileConstants.LeftSchoolDate,
                DeceasedFlag = ProfileConstants.DeceasedFlag,
                ActiveFlag = ProfileConstants.ActiveFlag,
                CountryOfBirthCode = ProfileConstants.CountryOfBirthCode,
                CreatedOn = ProfileConstants.Createdon,
                CreatedBy = ProfileConstants.Createdby,
                UpdatedOn = ProfileConstants.Updatedon,
                UpdatedBy = ProfileConstants.Updatedby,
                LanguageCode = ProfileConstants.LanguageCode,
                PreferredContactTypeCode = ProfileConstants.PreferredContactType.ToString(),
                VisaNumber = ProfileConstants.VisaNumber,
                CustomerReferenceNumber = ProfileConstants.CustomerReferenceNumber
            };
            profile.Addresses.Add(postal);
            profile.Addresses.Add(RestAddress);
            profile.Phones.Add(new Phone() {PhoneNumber = ProfileConstants.Phone1, PhoneTypeCode = PhoneType.PHONE1.ToString()});
            profile.Phones.Add(new Phone() {PhoneNumber = ProfileConstants.Phone2, PhoneTypeCode = PhoneType.PHONE2.ToString()});
            profile.PriorQualifications.Add(ProfileConstants.Qualification);
            profile.PriorApprenticeshipQualifications.Add(ProfileConstants.PriorApprenticeshipQualification);
            profile.USIs.Add(new ApprenticeUSI() {USI = ProfileConstants.USI, ActiveFlag = true, USIStatus = "test"});
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
            model.BirthDate.Should().Be(ProfileConstants.Birthdate);
            model.EmailAddress.Should().Be(ProfileConstants.Emailaddress);
            model.CreatedOn.Should().Be(ProfileConstants.Createdon);
            model.UpdatedOn.Should().Be(ProfileConstants.Updatedon);
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
            model.PreferredName.Should().Be(ProfileConstants.PreferredName);
            model.SelfAssessedDisabilityCode.Should().Be(ProfileConstants.SelfAssessedDisabilityCode);
            model.LanguageCode.Should().Be(ProfileConstants.LanguageCode);
            model.PreferredContactTypeCode.Should().Be(ProfileConstants.PreferredContactType.ToString());
            model.HighestSchoolLevelCode.Should().Be(ProfileConstants.HighestSchoolLevelCode);
            model.LeftSchoolDate.Should().Be(ProfileConstants.LeftSchoolDate);
            model.VisaNumber.Should().Be(ProfileConstants.VisaNumber);
            model.USIVerificationResult.USI.Should().Be(ProfileConstants.USI);
            model.USIVerificationResult.USIStatus.Should().Be("test");
            model.CRNViewFlag.Should().Be(ProfileConstants.CustomerReferenceNumber != null);
            model.Phone1.Should().Be(ProfileConstants.Phone1);
            model.Phone2.Should().Be(ProfileConstants.Phone2);
        }

        [TestMethod]
        public void SetAddressToNullIfNoAddress()
        {
            profile.Addresses.Clear();
            model = new ProfileModel(profile);
            model.PostalAddress.Should().BeNull();
            model.ResidentialAddress.Should().BeNull();
        }

        [TestMethod]
        public void SetPhonesToNullIfNoPhones()
        {
            profile.Phones.Clear();
            model = new ProfileModel(profile);
            model.Phone1.Should().BeNull();
            model.Phone2.Should().BeNull();
        }
    }

    #endregion
}