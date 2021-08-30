using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
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
                Phone1 = ProfileConstants.Phone1,
                Phone2 = ProfileConstants.Phone2,
                ResidentialAddress = ProfileConstants.ResidentialAddress,
                PostalAddress = ProfileConstants.PostalAddress,
                PreferredContactTypeCode = ProfileConstants.PreferredContactType.ToString(),
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
                .ReturnsAsync(new ValidationExceptionBuilder());
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
            profile.BirthDate.Should().Be(message.BirthDate.Value);
            profile.GenderCode.Should().Contain(message.GenderCode);
            profile.ProfileTypeCode.Should().Be(message.ProfileType);
        }

        [TestMethod]
        public void SetsContactDetails()
        {
            profile.EmailAddress.Should().Be(message.EmailAddress);            
            profile.Phones.Select(c => c.PhoneNumber).Should().Contain(message.Phone1);
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
            profile.PreferredContactTypeCode.Should().Be(message.PreferredContactTypeCode);
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

            profile.Phones.Clear();
            profile.Phones.Add(new Phone()
            {
                PhoneNumber = phonenumber1,
                PhoneTypeCode = PhoneType.PHONE1.ToString()
            });
            profile.Phones.Add(new Phone()
            {
                PhoneNumber = phonenumber2,
                PhoneTypeCode = PhoneType.PHONE2.ToString()
            });

            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                Phone1 = ProfileConstants.Phone1,
                Phone1CountryCode = ProfileConstants.Phone1CountryCode,
                Phone2 = ProfileConstants.Phone2,
            };

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            profile.Phones.Count.Should().Be(2);            
            profile.Phones.Where(x => x.PhoneTypeCode == PhoneType.PHONE1.ToString()).FirstOrDefault().PhoneNumber.Should().Be(ProfileConstants.Phone1);
            profile.Phones.Where(x => x.PhoneTypeCode == PhoneType.PHONE2.ToString()).FirstOrDefault().PhoneNumber.Should().Be(ProfileConstants.Phone2);
        }

        [TestMethod]
        public async Task AddNewPhoneNumbers()
        {
            profile.Phones.Clear();            
            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,  
                Phone2CountryCode = ProfileConstants.Phone2CountryCode,
                Phone2 = ProfileConstants.Phone2,
            };

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            profile.Phones.Count.Should().Be(1);
            profile.Phones.Where(x => x.PhoneTypeCode == PhoneType.PHONE2.ToString()).FirstOrDefault().PhoneNumber.Should().Be(ProfileConstants.Phone2);
        }

        //[TestMethod]
        //public async Task UpdateExistingPhoneNumbers()
        //{
        //    //given
        //    var phonenumber1 = "0212345678";
        //    var phonenumber2 = "0412345678";
        //    var phonenumber3 = "0412345670";

        //    profile.Phones.Clear();
        //    profile.Phones.Add(new Phone()
        //    {
        //        PhoneNumber = phonenumber1,
        //        PreferredPhoneFlag = true,
        //        Id = 1
        //    });
        //    profile.Phones.Add(new Phone()
        //    {
        //        PhoneNumber = phonenumber2,
        //        PreferredPhoneFlag = false,
        //        Id = 2
        //    });

        //    List<UpdatePhoneNumberMessage> UpdatedPhoneNumbers = new List<UpdatePhoneNumberMessage>()
        //    {
        //        new UpdatePhoneNumberMessage() {PhoneNumber = phonenumber2, Id = 1},
        //        new UpdatePhoneNumberMessage() {PhoneNumber = phonenumber3, PreferredPhoneFlag = true}
        //    };
        //    message = new UpdateProfileMessage
        //    {
        //        Surname = ProfileConstants.Surname,
        //        FirstName = ProfileConstants.Firstname,
        //        BirthDate = ProfileConstants.Birthdate,
        //        ProfileType = ProfileConstants.Profiletype,
        //        GenderCode = ProfileConstants.GenderCode,
        //        PhoneNumbers = UpdatedPhoneNumbers
        //    };

        //    //when
        //    profile = await ClassUnderTest.Update(profile, message);

        //    //then
        //    profile.Phones.Count.Should().Be(2);
        //    //Id:1 should have been updated
        //    profile.Phones.Where(x => x.Id == 1).FirstOrDefault().PhoneNumber.Should().Be(phonenumber2);
        //    //Id:2 should have been removed
        //    profile.Phones.Where(x => x.Id == 2).FirstOrDefault().Should().Be(null);
        //}

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
            profile.USIs.Add(new ApprenticeUSI {USI = "currentUSI", ActiveFlag = true});
            message = new UpdateProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                ProfileType = ProfileConstants.Profiletype,
                USI = "updatedUSI",
                USIChangeReason = "Incorrect USI entered"
            };
            Container.GetMock<IProfileValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<Profile>()))
                .ReturnsAsync(new ValidationExceptionBuilder());
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
        public async Task ShouldAddNewUSIWithChangeReason()
        {
            //given
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname, GenderCode = ProfileConstants.GenderCode};
            profile.USIs.Clear();
            profile.USIs.Add(new ApprenticeUSI {USI = "currentUSI", ActiveFlag = true, USIVerifyFlag = true});

            //when
            profile = await ClassUnderTest.Update(profile, message);

            //then
            ApprenticeUSI profileUSI = profile.USIs.Where(x => x.ActiveFlag == true).SingleOrDefault();
            profileUSI.USIChangeReason.Should().Be("Incorrect USI entered");
        }

        [TestMethod]
        public async Task SetExistingUSIFlagToFalseIfNoUSI()
        {
            //given
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname, GenderCode = ProfileConstants.GenderCode};
            profile.USIs.Clear();
            profile.USIs.Add(new ApprenticeUSI {USI = "currentUSI", ActiveFlag = true});
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
        private DateTime? deceasedDate;

        protected override void Given()
        {
            profile = new Profile {Surname = ProfileConstants.Surname, FirstName = ProfileConstants.Firstname};
            deceasedDate = DateTime.Now.AddYears(-1).Date;
        }

        protected override void When()
        {
            ClassUnderTest.UpdateDeceasedFlag(profile, true, deceasedDate);
        }

        [TestMethod]
        public void ShouldSetDeceasedDetails()
        {
            profile.DeceasedFlag.Should().Be(true);
            profile.DeceasedDate.Should().Be(deceasedDate);
            profile.ActiveFlag.Should().Be(false);
            profile.InactiveDate.Should().HaveValue();
        }

        [TestMethod]
        public void ShouldSetInactiveDateToNullIfNotDeceased()
        {
            profile.DeceasedFlag = false;
            ClassUnderTest.UpdateDeceasedFlag(profile, false, deceasedDate);
            profile.InactiveDate.Should().NotHaveValue();
        }

        [TestMethod]
        public void ShouldNotUpdateInactiveDateIfInactiveDateIsSet()
        {
            profile.DeceasedFlag = true;
            profile.InactiveDate = DateTime.Now.AddDays(-1).Date;
            ClassUnderTest.UpdateDeceasedFlag(profile, true, deceasedDate);
            profile.InactiveDate.Should().Be(DateTime.Now.AddDays(-1).Date);
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