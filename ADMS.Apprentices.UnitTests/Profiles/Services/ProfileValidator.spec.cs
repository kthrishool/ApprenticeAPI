using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingAProfile

    [TestClass]
    public class WhenValidatingAProfile : GivenWhenThen<ProfileValidator>
    {
        private Profile validProfile;
        private Profile invalidProfile;
        private Profile profile;
        protected override void Given()
        {
            validProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress,
                ProfileTypeCode = ProfileConstants.Profiletype
            };
            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = DateTime.Now.AddYears(-10),
                EmailAddress = ProfileConstants.RandomString(64) + "@" + ProfileConstants.RandomString(256) + "." + ProfileConstants.RandomString(50),
                ProfileTypeCode = ProfileConstants.Profiletype,
            };

            Container.GetMock<IUSIValidator>()
                .Setup(r => r.Validate(It.IsAny<Profile>()))
                .Returns(() => new ValidationExceptionBuilder());

            Container.GetMock<IAddressValidator>()
                .Setup(r => r.ValidateAsync(It.IsAny<IAddressAttributes>()))
                .ReturnsAsync(new ValidationExceptionBuilder());
            
            Container.GetMock<IPhoneValidator>()
                .Setup(a => a.ValidatePhonewithType(It.IsAny<ValidationExceptionBuilder>(), It.IsAny<Phone>()));
        }

        [TestMethod]
        public async Task DoesNothingIfTheProfileIsValid()
        {
            (await ClassUnderTest.ValidateAsync(validProfile)).ThrowAnyExceptions();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfAgeIsLessThan12()
        {
            validProfile.ProfileTypeCode = null;
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfProfileTypeIsNull()
        {
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(invalidProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfDOBIsInvalid()
        {
            validProfile.BirthDate = new DateTime(0001, 01, 01);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfNoContact()
        {
            validProfile.EmailAddress = null;
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfNoPhone()
        {
            validProfile.EmailAddress = null;
            validProfile.Phones.Clear();
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfTypeIsInvalid()
        {
            validProfile.ProfileTypeCode = "Invalid";
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }


        #region LeftSchoolDetailsValidation

        [TestMethod]
        public void ThrowsValidationExceptionIfLeftSchoolIsLessThanDOB()
        {
            validProfile.LeftSchoolDate = ProfileConstants.Birthdate.AddDays(-1);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfLeftSchoolIsGreaterThanToday()
        {
            validProfile.LeftSchoolDate = DateTime.Today.AddDays(1);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void DoNothingIsDayLeftSchoolIsValid()
        {
            validProfile.LeftSchoolDate = ProfileConstants.Birthdate.AddYears(10);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }        

        [TestMethod]
        public async Task DoNothingIsDayLeftSchoolIsEqual()
        {
            validProfile.LeftSchoolDate = ProfileConstants.Birthdate;
            (await ClassUnderTest.ValidateAsync(validProfile)).ThrowAnyExceptions();
        }

        #endregion

        
        [TestMethod]
        public async Task DoesNothingIfEmailIsEmpty()
        {
            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = DateTime.Now.AddYears(-14),
                ProfileTypeCode = ProfileConstants.Profiletype,                           
            };
            invalidProfile.Phones.Add(new Phone()
            {
                PhoneNumber = "0411111111",
                PhoneTypeCode = "MOBILE",
                PreferredPhoneFlag = true
            });

            (await ClassUnderTest.ValidateAsync(invalidProfile)).ThrowAnyExceptions();
        }

        private void GetsTheValidationExceptionIfEmailIsInvalid(string EmailAddress)
        {
            validProfile.EmailAddress = EmailAddress;

            // ExecuteTest(validProfile);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void GetValidationExceptionForEmails()
        {
            // onlyDomainName
            GetsTheValidationExceptionIfEmailIsInvalid("@.com");
            // onlyDomainName
            GetsTheValidationExceptionIfEmailIsInvalid("@test.com");
            // No .com
            GetsTheValidationExceptionIfEmailIsInvalid("turtjfhfhgfhg@jkhjkhjkhjk");
            // No Domain Name
            GetsTheValidationExceptionIfEmailIsInvalid("ghjghjg@.comjjj");
            // No @symbol
            GetsTheValidationExceptionIfEmailIsInvalid("ghjghjg.comjjj");
            // ..
            GetsTheValidationExceptionIfEmailIsInvalid("abc@gmail..comjjj");
        }


        [TestMethod]
        public void GetsTheValidationExceptionFromTheExceptionFactory()
        {
            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                ProfileTypeCode = ProfileConstants.Profiletype,
                BirthDate = DateTime.Now.AddYears(-10),
                EmailAddress = ProfileConstants.RandomString(64) + "@" + ProfileConstants.RandomString(256) + "." + ProfileConstants.RandomString(50)
            };

            ExecuteTest(invalidProfile);
        }

        private void ExecuteTest(Profile profiledata)
        {
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profiledata)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        /// <summary>
        /// check if the ProfileType Is invalid
        /// </summary>
        [TestMethod]
        public void ThrowsValidationExceptionIfProfileTypeIsInvalid()
        {
            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = DateTime.Now.AddYears(-10),
                EmailAddress = ProfileConstants.RandomString(64) + "@" + ProfileConstants.RandomString(256) + "." + ProfileConstants.RandomString(50),
                ProfileTypeCode = "app"
            };

            ExecuteTest(invalidProfile);
        }


        #region PhoneNumber

        [TestMethod]
        public void DoesNothingIfForPhonePositiveTesting()
        {
            validProfile.Phones.Clear(); 
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void DoesNothingIfPhoneIsNull()
        {
            // Phone is null
            Phone phone = null;
            validProfile.Phones.Add(phone);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void DoesNothingIfPhoneNumberIsEmpty()
        {
            var phone = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = ""};
            validProfile.Phones.Add(phone);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void NoValidationExceptionIfThePhoneNumberIsValid()
        {
            var phones = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = "0212345678"};

            validProfile.Phones.Add(phones);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void DoesNothingIfThePhoneNumberIsNull()
        {
            var phones = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = "0212345678"};

            validProfile.Phones.Add(phones);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        #endregion

        /// <summary>
        /// Insert a profile record and check if the email has been updated .
        /// </summary>
        [TestMethod]
        public async Task DoesNothingIfUSIIsEmpty()
        {
            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = DateTime.Now.AddYears(-14),
                ProfileTypeCode = ProfileConstants.Profiletype,
                EmailAddress = ProfileConstants.Emailaddress
            };

            (await ClassUnderTest.ValidateAsync(invalidProfile)).ThrowAnyExceptions();
        }

        private void ExecuteUSITest(string USI, Boolean ActiveFlag, string USIStatus)
        {
            validProfile.USIs.Add( new ApprenticeUSI() {USI = USI, ActiveFlag = ActiveFlag, USIStatus = USIStatus});
            Container.GetMock<IUSIValidator>()
                .Setup(r => r.Validate(validProfile))
                .Returns(() => new ValidationExceptionBuilder());


            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions());
        }

        /// <summary>
        /// Insert a profile record and check if the email has been updated .
        /// </summary>
        [TestMethod]
        public void ThrowsValidationExceptionIfUSIIsNull()
        {
            ExecuteUSITest("", true, "test");
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfUSIIsInvalid()
        {
            ExecuteUSITest("12131", true, "test");
        }

        [TestMethod]
        public void DoesNothingIfUSIIsValid()
        {
            validProfile.USIs.Clear();
            validProfile.USIs.Add( new ApprenticeUSI() {USI = "23456789D1", ActiveFlag = true, USIStatus = "test"});

            Container.GetMock<IUSIValidator>()
                .Setup(r => r.Validate(validProfile));

            // ExecuteTest(validProfile);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Invoke();
        }

        #region Address Validation

        [TestMethod]
        public void DONothingIfAddressIsNotSupplied()
        {
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }

        [TestMethod]
        public void DONothingIfOneAddressIsSupplied()
        {
            var validAddress = new Address()
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
            validProfile.Addresses.Add(validAddress);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }


        [TestMethod]
        public void DONothingIfTwoAddressIsSupplied()
        {
            var validAddress = new Address()
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
            var secondAddress = new Address()
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
            validProfile.Addresses.Add(validAddress);
            validProfile.Addresses.Add(secondAddress);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowExceptionIfSecondAddressIsNotValid()
        {
            var validAddress = new Address()
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
            var secondAddress = new Address()
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
            validProfile.Addresses.Add(validAddress);
            validProfile.Addresses.Add(secondAddress);
            var validationBuilder = new ValidationExceptionBuilder();
            validationBuilder.AddException(ValidationExceptionType.AddressRecordNotFound);
            Container.GetMock<IAddressValidator>().Setup(av => av.ValidateAsync(secondAddress)).ReturnsAsync(validationBuilder);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(validProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowBubbledExceptionIfAddressValidatorsThowsOne()
        {
            var validAddress = new Address()
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
            var secondAddress = new Address()
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
            validProfile.Addresses.Add(validAddress);
            validProfile.Addresses.Add(secondAddress);
            var validationBuilder = new ValidationExceptionBuilder();
            validationBuilder.AddException(ValidationExceptionType.AddressRecordNotFound);
            Container.GetMock<IAddressValidator>().Setup(av => av.ValidateAsync(secondAddress)).Throws(new NotSupportedException());
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(validProfile)))
                .Should().Throw<NotSupportedException>();
        }
        #endregion

        #region PreferredContactType
        [TestMethod]
        public void DoesNothingIfPreferredCodeTypeIsValid()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = PreferredContactType.MOBILE.ToString();
            profile.Phones.Add(new Phone() { PhoneNumber = "0411111111" });
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(profile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenMobileContactTypeIsInvalid()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = PreferredContactType.MOBILE.ToString();
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }        

        [TestMethod]
        public void ThrowValidationExceptionWhenContactTypeIsNotValidCode()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = "test";
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void DoNothingWhenPhoneContactTypeIsValid()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = PreferredContactType.PHONE.ToString();
            profile.Phones.Add(new Phone() { PhoneNumber = "0211111111" });
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionWhenPhoneContactTypeIsNotValid()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = PreferredContactType.PHONE.ToString();
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void DoNothingWhenEmailContactTypeIsValid()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = PreferredContactType.EMAIL.ToString();
            profile.EmailAddress = ProfileConstants.Emailaddress;
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionWhenEmailContactTypeIsInvalid()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = PreferredContactType.EMAIL.ToString();
            profile.EmailAddress = null;
            profile.Phones.Add(new Phone() { PhoneNumber = "0211111111" });
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void DoNothingWhenMailContactTypeIsValid()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = PreferredContactType.MAIL.ToString();
            profile.Addresses.Add(new Address()
            {
                StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
                Postcode = ProfileConstants.ResidentialAddress.Postcode,
                Locality = ProfileConstants.ResidentialAddress.Locality,
                StateCode = ProfileConstants.ResidentialAddress.StateCode
            });
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionWhenMailContactTypeIsInvalid()
        {
            profile = GetNewProfile();
            profile.PreferredContactTypeCode = PreferredContactType.MAIL.ToString();            
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }
        
        #endregion
        private Profile GetNewProfile()
        {
            return new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress,
                ProfileTypeCode = ProfileConstants.Profiletype
            };
        }
    }
   
    #endregion
}