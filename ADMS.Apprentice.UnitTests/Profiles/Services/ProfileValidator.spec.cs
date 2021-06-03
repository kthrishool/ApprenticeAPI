﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenValidatingAProfile

    [TestClass]
    public class WhenValidatingAProfile : GivenWhenThen<ProfileValidator>
    {
        private Profile validProfile;
        private Profile invalidProfile;
        private ValidationException validationException;

        protected override void Given()
        {
            validProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress,
                ProfileTypeCode = ProfileConstants.Profiletype,
                PreferredContactType = ProfileConstants.PreferredContactType.ToString(),
            };
            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = DateTime.Now.AddYears(-10),
                EmailAddress = ProfileConstants.RandomString(64) + "@" + ProfileConstants.RandomString(256) + "." + ProfileConstants.RandomString(50),
                ProfileTypeCode = ProfileConstants.Profiletype,
            };

            validationException = new ValidationException(null, (ValidationError) null);
            ChangeException(ValidationExceptionType.InvalidApprenticeAge);
            ChangeException(ValidationExceptionType.InvalidLeftSchoolYear);
            Container.GetMock<IQualificationValidator>()
                .Setup(r => r.ValidateAsync(It.IsAny<List<Qualification>>()))
                .ReturnsAsync(new List<Qualification>());
        }

        private void ChangeException(ValidationExceptionType exceptionMessage)
        {
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(exceptionMessage))
                .Returns(validationException);
        }


        [TestMethod]
        public async Task DoesNothingIfTheProfileIsValid()
        {
            await ClassUnderTest.ValidateAsync(validProfile);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfAgeIsLessThan12()
        {
            validProfile.ProfileTypeCode = null;
            ChangeException(ValidationExceptionType.InvalidApprenticeprofileType);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfProfileTypeIsNull()
        {
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(invalidProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfDOBIsInvalid()
        {
            validProfile.BirthDate = new DateTime(0001, 01, 01);
            ChangeException(ValidationExceptionType.InvalidDOB);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfNoContact()
        {
            validProfile.EmailAddress = null;
            ChangeException(ValidationExceptionType.MandatoryContact);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfNoPhone()
        {
            validProfile.EmailAddress = null;
            validProfile.Phones = null;
            ChangeException(ValidationExceptionType.MandatoryContact);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfTypeIsInvalid()
        {
            validProfile.ProfileTypeCode = "Invalid";
            ChangeException(ValidationExceptionType.InvalidApprenticeprofileType);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }


        #region LeftSchoolDetailsValidation

        [TestMethod]
        public void ThrowsValidationExceptionIfLeftSchoolIsLessThanDOB()
        {
            validProfile.LeftSchoolDate = ProfileConstants.Birthdate.AddDays(-1);
            ChangeException(ValidationExceptionType.InvalidLeftSchoolDetails);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfLeftSchoolIsGreaterThanToday()
        {
            validProfile.LeftSchoolDate = DateTime.Today.AddDays(1);
            ChangeException(ValidationExceptionType.InvalidLeftSchoolDetails);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void DoNothingIsDayLeftSchoolIsValid()
        {
            validProfile.LeftSchoolDate = ProfileConstants.Birthdate.AddYears(10);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().NotThrow<ValidationException>();
        }        

        [TestMethod]
        public async Task DoNothingIsDayLeftSchoolIsEqual()
        {
            validProfile.LeftSchoolDate = ProfileConstants.Birthdate;
            await ClassUnderTest.ValidateAsync(validProfile);
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
                Phones = new List<Phone> { new Phone()
                {
                    PhoneNumber = "0411111111",
                    PhoneTypeCode = "MOBILE",
                    PreferredPhoneFlag = true
                } }                
            };

            await ClassUnderTest.ValidateAsync(invalidProfile);
        }

        private void GetsTheValidationExceptionIfEmailIsInvalid(string EmailAddress)
        {
            ChangeException(ValidationExceptionType.InvalidEmailAddress);


            validProfile.EmailAddress = EmailAddress;

            // ExecuteTest(validProfile);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
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
                BirthDate = DateTime.Now.AddYears(-10),
                EmailAddress = ProfileConstants.RandomString(64) + "@" + ProfileConstants.RandomString(256) + "." + ProfileConstants.RandomString(50)
            };

            ExecuteTest(invalidProfile);
        }

        private void ExecuteTest(Profile profiledata)
        {
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(profiledata))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        /// <summary>
        /// check if the ProfileType Is invalid
        /// </summary>
        [TestMethod]
        public void ThrowsValidationExceptionIfProfileTypeIsInvalid()
        {
            ChangeException(ValidationExceptionType.InvalidApprenticeprofileType);


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
            // Phone Number is null
            var phones = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = "0212345678"};

            validProfile.Phones = null; //.Add(phones);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().NotThrow();
        }

        [TestMethod]
        public void DoesNothingIfPhoneIsNull()
        {
            // Phone is null
            Phone phone = null;
            validProfile.Phones.Add(phone);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().NotThrow();
        }

        [TestMethod]
        public void DoesNothingIfPhoneNumberIsEmpty()
        {
            var phone = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = ""};
            validProfile.Phones.Add(phone);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().NotThrow();
        }

        [TestMethod]
        public void NoValidationExceptionIfThePhoneNumberIsValid()
        {
            var phones = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = "0212345678"};

            validProfile.Phones.Add(phones);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().NotThrow();
        }

        [TestMethod]
        public void DoesNothingIfThePhoneNumberIsNull()
        {
            var phones = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = "0212345678"};

            validProfile.Phones.Add(phones);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().NotThrow();
        }

        [TestMethod]
        public async Task SetPreferredFlagOnlyOnOnePhone()
        {
            var phone1 = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = "0212345678", PreferredPhoneFlag = true};
            var phone2 = new Phone() {PhoneTypeCode = PhoneType.MOBILE.ToString(), PhoneNumber = "0404000000", PreferredPhoneFlag = true};

            validProfile.Phones.Add(phone1);
            validProfile.Phones.Add(phone2);
            validProfile = await ClassUnderTest.ValidateAsync(validProfile);
            validProfile.Phones.Where(x => x.PreferredPhoneFlag == true).Count().Should().Be(1);
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

            await ClassUnderTest.ValidateAsync(invalidProfile);
        }

        private void ExecuteUSITest(string USI, Boolean ActiveFlag, string USIStatus)
        {
            ChangeException(ValidationExceptionType.InvalidUSI);
            validProfile.USIs = new List<ApprenticeUSI>() {new ApprenticeUSI() {USI = USI, ActiveFlag = ActiveFlag, USIStatus = USIStatus}};
            Container.GetMock<IUSIValidator>()
                .Setup(r => r.Validate(validProfile))
                .Returns(false);


            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Equals(false);
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
            ChangeException(ValidationExceptionType.InvalidUSI);


            validProfile.USIs = new List<ApprenticeUSI>() {new ApprenticeUSI() {USI = "23456789D1", ActiveFlag = true, USIStatus = "test"}};

            Container.GetMock<IUSIValidator>()
                .Setup(r => r.Validate(validProfile))
                .Returns(false);

            // ExecuteTest(validProfile);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Equals(true);
        }

        #region Address Validation

        [TestMethod]
        public void DONothingIfAddressIsNotSupplied()
        {
            ClassUnderTest.Invoking(c => c.ValidateAsync(validProfile)).Should().NotThrow<ValidationException>();
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
            ClassUnderTest.Invoking(c => c.ValidateAsync(validProfile)).Should().NotThrow<ValidationException>();
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
            ClassUnderTest.Invoking(c => c.ValidateAsync(validProfile)).Should().NotThrow<ValidationException>();
        }

        #endregion
    }

    #endregion
}