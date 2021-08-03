using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenCreatingAProfile

    [TestClass]
    public class WhenCreatingAProfile : GivenWhenThen<ProfileCreator>
    {
        private Profile profile;
        private ProfileMessage message;

        protected override void Given()
        {
            message = new ProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress,
                ProfileType = ProfileConstants.Profiletype,
                PhoneNumbers = ProfileConstants.PhoneNumbers,
                ResidentialAddress = ProfileConstants.ResidentialAddress,
                PostalAddress = ProfileConstants.PostalAddress,
                IndigenousStatusCode = ProfileConstants.IndigenousStatusCode,
                SelfAssessedDisabilityCode = ProfileConstants.SelfAssessedDisabilityCode,
                CitizenshipCode = ProfileConstants.CitizenshipCode,
                GenderCode = ProfileConstants.GenderCode,
                InterpretorRequiredFlag = ProfileConstants.InterpretorRequiredFlag,
                LanguageCode = ProfileConstants.LanguageCode,
                PreferredContactType = ProfileConstants.PreferredContactType.ToString(),
                CountryOfBirthCode = ProfileConstants.CountryOfBirthCode,
                HighestSchoolLevelCode = ProfileConstants.HighestSchoolLevelCode,
                LeftSchoolDate = ProfileConstants.LeftSchoolDate,
                VisaNumber = ProfileConstants.VisaNumber,
                USI = ProfileConstants.USI
            };
            Container.GetMock<IProfileValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<Profile>()))
                .ReturnsAsync(new ValidationExceptionBuilder());
        }

        protected override async void When()
        {
            profile = await ClassUnderTest.CreateAsync(message);
        }

        [TestMethod]
        public void ShouldReturnProfile()
        {
            profile.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldAddTheProfileToTheDatabase()
        {
            Container.GetMock<IRepository>().Verify(r => r.Insert(profile));
        }

        [TestMethod]
        public void ShouldValidatesTheProfileRequest()
        {
            Container.GetMock<IProfileValidator>().Verify(r => r.ValidateAsync(profile));
        }

        [TestMethod]
        public void ShouldSetTheName()
        {
            profile.FirstName.Should().Be(message.FirstName);
            profile.Surname.Should().Be(message.Surname);
        }

        [TestMethod]
        public void ShouldSetTheBirthDate()
        {
            profile.BirthDate.Should().Be(message.BirthDate.Value);
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            profile.DeceasedFlag.Should().BeFalse();
            profile.ActiveFlag.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldSetProfileType()
        {
            profile.ProfileTypeCode.Should().Be(ProfileConstants.Profiletype);
        }

        /// <summary>
        /// Insert a profile record and check if the email has been updated .
        /// </summary>
        [TestMethod]
        public void ShouldSetEmailAddress()
        {
            profile.EmailAddress.Should().Be(message.EmailAddress);
        }

        [TestMethod]
        public void ShouldSetIndigenousStatusCode()
        {
            profile.IndigenousStatusCode.Should().Be(message.IndigenousStatusCode);
        }

        [TestMethod]
        public void ShouldSetDisabilityStatusCode()
        {
            profile.SelfAssessedDisabilityCode.Should().Be(message.SelfAssessedDisabilityCode);
        }

        [TestMethod]
        public void ShouldSetCitizenshipCode()
        {
            profile.CitizenshipCode.Should().Be(message.CitizenshipCode);
        }

        [TestMethod]
        public void ShouldSetInterpretorRequiredFlag()
        {
            profile.InterpretorRequiredFlag.Should().Be(message.InterpretorRequiredFlag);
        }


        [TestMethod]
        public void ShouldSetPhoneNumber()
        {
            profile.Phones.Select(c => c.PhoneNumber).Should().Contain(message.PhoneNumbers[0].PhoneNumber);
        }

        [TestMethod]
        public void ShouldSetResidentialAddress()
        {
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
                })
                .Should().Contain(message.ResidentialAddress);
        }

        [TestMethod]
        public void ShouldSetPostalAddress()
        {
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
                .Should().Contain(message.PostalAddress);
        }

        [TestMethod]
        public void ShouldSetGenderCode()
        {
            profile.GenderCode.Should().Contain(ProfileConstants.GenderCode);
        }

        [TestMethod]
        public void ShouldSetCountryofBirthCodeCode()
        {
            profile.CountryOfBirthCode.Should().Contain(ProfileConstants.CountryOfBirthCode);
        }

        [TestMethod]
        public void ShouldSetLanguageCode()
        {
            profile.LanguageCode.Should().Contain(ProfileConstants.LanguageCode);
        }

        #endregion

        [TestMethod]
        public void ShouldSetPreferredContactCode()
        {
            profile.PreferredContactType.Should().Contain(ProfileConstants.PreferredContactType.ToString());
        }

        [TestMethod]
        public void ShouldSetSchoolLevelCode()
        {
            profile.HighestSchoolLevelCode.Should().Contain(ProfileConstants.HighestSchoolLevelCode);
        }


        [TestMethod]
        public void ShouldSetLeftSchoolYearCode()
        {
            profile.LeftSchoolDate.Should().Be(ProfileConstants.LeftSchoolDate);
        }

        [TestMethod]
        public void ShouldSetVisaNumber()
        {
            profile.VisaNumber.Should().Contain(ProfileConstants.VisaNumber);
        }


        [TestMethod]
        public async Task PhonesShouldBeNullIfNoPhonesPassed()
        {
            message = new ProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                PhoneNumbers = null
            };
            profile = await ClassUnderTest.CreateAsync(message);
            profile.Phones.Should().BeEmpty();
        }


        [TestMethod]
        public async Task ShouldSetUSI()
        {
            message = new ProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                PhoneNumbers = null
            };
            profile = await ClassUnderTest.CreateAsync(message);
            profile.USIs.Select(c => c.USI = ProfileConstants.USI);
        }
    }
}