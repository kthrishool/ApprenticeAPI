﻿using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentice.Core.Services.Validators;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenUpdatingAProfile

    [TestClass]
    public class WhenUpdatigAProfile : GivenWhenThen<ProfileUpdater>
    {
        private Profile profile;
        private UpdateProfileMessage message;

        protected override void Given()
        {
            profile = new Profile();
            message = new UpdateProfileMessage(
                new BasicDetailsMessage
                {
                    Surname = ProfileConstants.Surname,
                    FirstName = ProfileConstants.Firstname,
                    BirthDate = ProfileConstants.Birthdate,
                    ProfileType = ProfileConstants.Profiletype,
                    GenderCode = ProfileConstants.GenderCode,
                },
                new ContactDetailsMessage
                {
                    EmailAddress = ProfileConstants.Emailaddress,
                    PhoneNumbers = ProfileConstants.PhoneNumbers,
                    ResidentialAddress = ProfileConstants.ResidentialAddress,
                    PostalAddress = ProfileConstants.PostalAddress,
                    PreferredContactType = ProfileConstants.PreferredContactType.ToString(),
                },
                new SchoolDetailsMessage
                {
                    HighestSchoolLevelCode = ProfileConstants.HighestSchoolLevelCode,
                    LeftSchoolMonthCode = ProfileConstants.LeftSchoolMonthCode,
                    LeftSchoolYearCode = ProfileConstants.LeftSchoolYearCode,
                },
                new OtherDetailsMessage
                {
                    IndigenousStatusCode = ProfileConstants.IndigenousStatusCode,
                    SelfAssessedDisabilityCode = ProfileConstants.SelfAssessedDisabilityCode,
                    CitizenshipCode = ProfileConstants.CitizenshipCode,
                    InterpretorRequiredFlag = ProfileConstants.InterpretorRequiredFlag,
                    LanguageCode = ProfileConstants.LanguageCode,
                    CountryOfBirthCode = ProfileConstants.CountryOfBirthCode,
                },
                new QualificationDetailsMessage
                {
                    Qualifications = ProfileConstants.Qualifications,
                }
            );
        }

        protected override async void When()
        {
            profile = await ClassUnderTest.Update(profile, message);
        }

        [TestMethod]
        public void SetsBasicDetails()
        {
            profile.FirstName.Should().Be(message.BasicDetails.FirstName);
            profile.Surname.Should().Be(message.BasicDetails.Surname);
            profile.BirthDate.Should().Be(message.BasicDetails.BirthDate);
            profile.GenderCode.Should().Contain(message.BasicDetails.GenderCode);
            profile.ProfileTypeCode.Should().Be(message.BasicDetails.ProfileType);
        }

        [TestMethod]
        public void SetsContactDetails()
        {
            profile.EmailAddress.Should().Be(message.ContactDetails.EmailAddress);
            profile.Phones.Count().Should().Be(message.ContactDetails.PhoneNumbers.Count());
            profile.Phones.Select(c => c.PhoneNumber).Should().Contain(message.ContactDetails.PhoneNumbers.Select(c => c.PhoneNumber));
            profile.Addresses.Where(c => c.AddressTypeCode == AddressType.RESD.ToString())
                .Select(c => new ProfileAddressMessage()
                {
                    Postcode = c.Postcode,
                    StateCode = c.StateCode,
                    SingleLineAddress = c.SingleLineAddress,
                    Locality = c.Locality,
                    StreetAddress1 = c.StreetAddress1,
                    StreetAddress2 = c.StreetAddress2,
                    StreetAddress3 = c.StreetAddress3
                }).Should().Contain(message.ContactDetails.ResidentialAddress);

            profile.Addresses.Where(c => c.AddressTypeCode == AddressType.POST.ToString())
                .Select(c => new ProfileAddressMessage()
                {
                    Postcode = c.Postcode,
                    StateCode = c.StateCode,
                    SingleLineAddress = c.SingleLineAddress,
                    Locality = c.Locality,
                    StreetAddress1 = c.StreetAddress1,
                    StreetAddress2 = c.StreetAddress2,
                    StreetAddress3 = c.StreetAddress3
                })
                .Should().Contain(message.ContactDetails.PostalAddress);
            profile.PreferredContactType.Should().Be(message.ContactDetails.PreferredContactType);
        }

        [TestMethod]
        public void SetsOtherDetails()
        {
            profile.InterpretorRequiredFlag.Should().Be(message.OtherDetails.InterpretorRequiredFlag);
            profile.CitizenshipCode.Should().Be(message.OtherDetails.CitizenshipCode);
            profile.SelfAssessedDisabilityCode.Should().Be(message.OtherDetails.SelfAssessedDisabilityCode);
            profile.IndigenousStatusCode.Should().Be(message.OtherDetails.IndigenousStatusCode);
            profile.LanguageCode.Should().Contain(message.OtherDetails.LanguageCode);
            profile.CountryOfBirthCode.Should().Contain(message.OtherDetails.CountryOfBirthCode);
        }

        [TestMethod]
        public void ShouldValidatesTheProfileRequest()
        {
            Container.GetMock<IProfileValidator>().Verify(r => r.ValidateAsync(profile));
        }
    }
    #endregion

    #region WhenUpdatingAProfileWithoutProvingAnyInfo
    [TestClass]
    public class WhenUpdatigAProfileWithoutProvidingAnyInfo : GivenWhenThen<ProfileUpdater>
    {
        private Profile profile;
        private UpdateProfileMessage message;

        protected override void Given()
        {
            profile = new Profile { Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname };
            message = new UpdateProfileMessage(null, null, null, null, null);
        }

        protected override async void When()
        {
            profile = await ClassUnderTest.Update(profile, message);
        }

        [TestMethod]
        public void ProfileShouldHaveNoChange()
        {
            profile.Should().Be(profile);
        }
    }
    #endregion
}