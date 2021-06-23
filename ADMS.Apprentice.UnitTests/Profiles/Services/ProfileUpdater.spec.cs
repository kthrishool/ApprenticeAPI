using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Adms.Shared.Exceptions;

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
            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                GenderCode = ProfileConstants.GenderCode,

                EmailAddress = ProfileConstants.Emailaddress,
                PhoneNumbers = ProfileConstants.UpdatedPhoneNumbers,
                ResidentialAddress = ProfileConstants.ResidentialAddress,
                PostalAddress = ProfileConstants.PostalAddress,
                PreferredContactType = ProfileConstants.PreferredContactType.ToString(),
                HighestSchoolLevelCode = ProfileConstants.HighestSchoolLevelCode,
                LeftSchoolDate = ProfileConstants.LeftSchoolDate,
                IndigenousStatusCode = ProfileConstants.IndigenousStatusCode,
                SelfAssessedDisabilityCode = ProfileConstants.SelfAssessedDisabilityCode,
                CitizenshipCode = ProfileConstants.CitizenshipCode,
                InterpretorRequiredFlag = ProfileConstants.InterpretorRequiredFlag,
                LanguageCode = ProfileConstants.LanguageCode,
                CountryOfBirthCode = ProfileConstants.CountryOfBirthCode
            };
            Container.GetMock<IProfileValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<Profile>()))
                .ReturnsAsync(new ValidationExceptionBuilder(Container.GetMock<IExceptionFactory>().Object));
        }

        protected override async void When()
        {
            profile = await ClassUnderTest.Update(profile, message);
        }

        [TestMethod]
        public void SetsBasicDetails()
        {
            profile.FirstName.Should().Be(message.FirstName);
            profile.Surname.Should().Be(message.Surname);
            profile.BirthDate.Should().Be(message.BirthDate);
            profile.GenderCode.Should().Contain(message.GenderCode);
            profile.ProfileTypeCode.Should().Be(message.ProfileType);
        }

        [TestMethod]
        public void SetsContactDetails()
        {
            profile.EmailAddress.Should().Be(message.EmailAddress);
            profile.Phones.Count().Should().Be(message.PhoneNumbers.Count());
            profile.Phones.Select(c => c.PhoneNumber).Should().Contain(message.PhoneNumbers.Select(c => c.PhoneNumber));
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
                }).Should().Contain(message.ResidentialAddress);

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
            profile.PreferredContactType.Should().Be(message.PreferredContactType);
        }

        [TestMethod]
        public void SetsOtherDetails()
        {
            profile.InterpretorRequiredFlag.Should().Be(message.InterpretorRequiredFlag);
            profile.CitizenshipCode.Should().Be(message.CitizenshipCode);
            profile.SelfAssessedDisabilityCode.Should().Be(message.SelfAssessedDisabilityCode);
            profile.IndigenousStatusCode.Should().Be(message.IndigenousStatusCode);
            profile.LanguageCode.Should().Contain(message.LanguageCode);
            profile.CountryOfBirthCode.Should().Contain(message.CountryOfBirthCode);
        }

        [TestMethod]
        public async Task UpdateExistingAddresses()
        {
            //given
            profile.Phones.Clear();
            profile.Addresses.Clear();
            profile.Addresses.Add(new Address()
            {
                StreetAddress1 = "street address 1",
                Locality = "Locality1",
                StateCode = "ACT",
                Postcode = "2615",
                AddressTypeCode = AddressType.RESD.ToString(),
            });
            profile.Addresses.Add(new Address()
            {
                StreetAddress1 = "post street address 1",
                Locality = "Locality2",
                StateCode = "ACT",
                Postcode = "2615",
                AddressTypeCode = AddressType.POST.ToString(),
            });

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            profile.Addresses.Where(c => c.AddressTypeCode == AddressType.POST.ToString()).SingleOrDefault().StreetAddress1.Should().NotBe("post street address 1");
            profile.Addresses.Where(c => c.AddressTypeCode == AddressType.RESD.ToString()).SingleOrDefault().StreetAddress1.Should().NotBe("street address 1");
        }

        [TestMethod]
        public async Task NoAddressesIfNoAddressPassed()
        {
            //given
            profile.Addresses.Clear();
            profile.Addresses.Add(new Address()
            {
                StreetAddress1 = "street address 1",
                Locality = "Locality1",
                StateCode = "ACT",
                Postcode = "2615",
                AddressTypeCode = AddressType.RESD.ToString(),
            });
            profile.Addresses.Add(new Address()
            {
                StreetAddress1 = "post street address 1",
                Locality = "Locality2",
                StateCode = "ACT",
                Postcode = "2615",
                AddressTypeCode = AddressType.POST.ToString(),
            });

            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                GenderCode = ProfileConstants.GenderCode
            };

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            profile.Addresses.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task UpdateExistingPhoneNumbers()
        {
            //given
            var phonenumber1 = "0212345678";
            var phonenumber2 = "0412345678";
            var phonenumber3 = "0412345670";

            profile.Phones.Clear();
            profile.Phones.Add(new Phone()
            {
                PhoneNumber = phonenumber1,
                PreferredPhoneFlag = true,
                Id = 1
            });
            profile.Phones.Add(new Phone()
            {
                PhoneNumber = phonenumber2,
                PreferredPhoneFlag = false,
                Id = 2
            });

            List<UpdatePhoneNumberMessage> UpdatedPhoneNumbers = new List<UpdatePhoneNumberMessage>()
            {
                new UpdatePhoneNumberMessage() {PhoneNumber = phonenumber2, Id = 1},
                new UpdatePhoneNumberMessage() {PhoneNumber = phonenumber3, PreferredPhoneFlag = true}
            };
            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                GenderCode = ProfileConstants.GenderCode,
                PhoneNumbers = UpdatedPhoneNumbers
            };

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            profile.Phones.Count.Should().Be(2);
            //Id:1 should have been updated
            profile.Phones.Where(x => x.Id == 1).FirstOrDefault().PhoneNumber.Should().Be(phonenumber2);
            //Id:2 should have been removed
            profile.Phones.Where(x => x.Id == 2).FirstOrDefault().Should().Be(null);
        }

        [TestMethod]
        public async Task VerifyUsiIfFirstNameChanged()
        {
            //given
            profile.FirstName = "firstName";

            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                GenderCode = ProfileConstants.GenderCode,
                USI = ProfileConstants.USI
            };

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            Container.GetMock<IUSIVerify>().Verify(r => r.Verify(profile));
        }

        [TestMethod]
        public async Task VerifyUsiIfBirthDateChanged()
        {
            //given            
            profile.BirthDate = DateTime.Now.AddYears(-30);

            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                GenderCode = ProfileConstants.GenderCode,
                USI = ProfileConstants.USI
            };

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            Container.GetMock<IUSIVerify>().Verify(r => r.Verify(profile));
        }

        [TestMethod]
        public async Task VerifyUsiIfSurnameNameChanged()
        {
            //given            
            profile.Surname = "surName";

            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                GenderCode = ProfileConstants.GenderCode,
                USI = ProfileConstants.USI
            };

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            Container.GetMock<IUSIVerify>().Verify(r => r.Verify(profile));
        }

        [TestMethod]
        public void ShouldValidatesTheProfileRequest()
        {
            Container.GetMock<IProfileValidator>().Verify(r => r.ValidateAsync(profile));
        }
    }

    #endregion

    #region WhenUpdatingUSI

    [TestClass]
    public class WhenUpdatingUSI : GivenWhenThen<ProfileUpdater>
    {
        private Profile profile;
        private UpdateProfileMessage message;

        protected override void Given()
        {
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname, GenderCode = ProfileConstants.GenderCode};
            profile.USIs = new List<ApprenticeUSI>() {new ApprenticeUSI {USI = "currentUSI", ActiveFlag = true}};
            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                USI = "updatedUSI"
            };
            Container.GetMock<IProfileValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<Profile>()))
                .ReturnsAsync(new ValidationExceptionBuilder(Container.GetMock<IExceptionFactory>().Object));
        }

        protected override async void When()
        {
            profile = await ClassUnderTest.Update(profile, message);
        }

        [TestMethod]
        public void ShouldUpdateUSI()
        {
            profile.USIs.Count.Should().Be(2);
            profile.USIs.Where(x => x.ActiveFlag == true).SingleOrDefault().USI.Should().Be(message.USI);
        }

        [TestMethod]
        public async Task ShouldAddNewUSI()
        {
            //given
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname, GenderCode = ProfileConstants.GenderCode};
            profile.USIs.Clear();

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            profile.USIs.Count.Should().Be(1);
        }

        [TestMethod]
        public async Task SetExistingUSIFlagToFalseIfNoUSI()
        {
            //given
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname, GenderCode = ProfileConstants.GenderCode};
            profile.USIs = new List<ApprenticeUSI>() {new ApprenticeUSI {USI = "currentUSI", ActiveFlag = true}};
            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype
            };

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            profile.USIs.Where(x => x.ActiveFlag == true).SingleOrDefault().Should().Be(null);
        }
    }

    #endregion

    #region WhenUpdatingDeceasedFlag

    [TestClass]
    public class WhenUpdatingDeceasedFlag : GivenWhenThen<ProfileUpdater>
    {
        private Profile profile;

        protected override void Given()
        {
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname};
        }

        protected override void When()
        {
            ClassUnderTest.UpdateDeceasedFlag(profile, true);
        }

        [TestMethod]
        public void DeceasedFlagShouldBeTrue()
        {
            profile.DeceasedFlag.Should().Be(true);
        }
    }

    #endregion

    #region WhenAdminUpdateSpecialAttributes

    [TestClass]
    public class WhenAdminUpdates : GivenWhenThen<ProfileUpdater>
    {
        private Profile profile;
        private AdminUpdateMessage message;

        protected override void Given()
        {
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname, DeceasedFlag = true};
            message = new AdminUpdateMessage {DeceasedFlag = false};
        }

        protected override void When()
        {
            ClassUnderTest.Update(profile, message);
        }

        [TestMethod]
        public void DeceasedFlagShouldBeFlase()
        {
            profile.DeceasedFlag.Should().Be(false);
        }
    }

    #endregion


    #region WhenServiceAustraliaUpdateApprentice`Attributes

    [TestClass]
    public class WhenServiceAustraliaUpdates : GivenWhenThen<ProfileUpdater>
    {
        private Profile profile;
        private ServiceAustraliaUpdateMessage message;

        protected override void Given()
        {
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname, DeceasedFlag = true};
            message = new ServiceAustraliaUpdateMessage {CustomerReferenceNumber = ProfileConstants.CustomerReferenceNumber};
        }

        protected override void When()
        {
            ClassUnderTest.UpdateCRN(profile, message);
        }

        [TestMethod]
        public void ThrowExceptionWhenCRNExceedsLength()
        {
            profile.CustomerReferenceNumber.Should().Be(ProfileConstants.CustomerReferenceNumber);
        }
    }

    #endregion
}