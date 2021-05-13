using System;
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
        public void ThrowsValidationExceptionIfLeftSchoolYearIsInvalid()
        {
            //should accept year between 1900 and current year
            validProfile.LeftSchoolYear = 1800;
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfLeftSchoolMonthIsInvalid()
        {
            validProfile.LeftSchoolYear = 2000;
            validProfile.LeftSchoolMonthCode = "Invalid";
            ChangeException(ValidationExceptionType.InvalidMonthCode);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfLeftSchoolMonthIsMissing()
        {
            validProfile.LeftSchoolYear = 2000;
            validProfile.LeftSchoolMonthCode = null;
            ChangeException(ValidationExceptionType.InvalidLeftSchoolDetails);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfLeftSchoolYearIsMissing()
        {
            validProfile.LeftSchoolYear = null;
            validProfile.LeftSchoolMonthCode = "JAN";
            ChangeException(ValidationExceptionType.InvalidLeftSchoolDetails);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public async Task SetsLeftSchoolDateIfLeftSchoolMonthYearIsValid()
        {
            validProfile.LeftSchoolYear = 2000;
            validProfile.LeftSchoolMonthCode = "JAN";
            await ClassUnderTest.ValidateAsync(validProfile);
            validProfile.LeftSchoolDate.Should().Be(new DateTime(2000, 1, 1));
        }

        #endregion

        /// <summary>
        /// Insert a profile record and check if the email has been updated .
        /// </summary>
        [TestMethod]
        public async Task DoesNothingIfEmailIsEmpty()
        {
            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = DateTime.Now.AddYears(-14),
                ProfileTypeCode = ProfileConstants.Profiletype
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

        private void PhoneContainerError(string phones)
        {
            var number = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = phones};

            ChangeException(ValidationExceptionType.InvalidPhoneNumber);


            validProfile.Phones.Add(number);
            ExecuteTest(validProfile);
        }

        private async void PhoneContainerPositive(string phones, Boolean needsConversion)
        {
            var number = new Phone() {PhoneTypeCode = PhoneType.LANDLINE.ToString(), PhoneNumber = phones};

            validProfile.Phones.Add(number);
            await ClassUnderTest.ValidateAsync(validProfile);
            if (needsConversion)
                validProfile.Phones.Select(c => c.PhoneNumber).Should().NotContain(phones);
            else
                validProfile.Phones.Select(c => c.PhoneNumber).Should().Contain(phones);
        }

        [TestMethod]
        public void ThrowExceptonforInValidPhoneNumbers()
        {
            ChangeException(ValidationExceptionType.InvalidPhoneNumber);
            // total Lenght is Less than 10 chars
            PhoneContainerError("021234567");
            // Area code is not valid 
            PhoneContainerError("9911234567");
            //Phone Number With Extension
            PhoneContainerError("0212457896#1234");
        }


        [TestMethod]
        public void DoNothingWhenaValidPhoneNumberisEntered()
        {
            // total Lenght is Less than 10 chars
            PhoneContainerPositive("+61212457896", true);
            // Area code is not valid 
            PhoneContainerPositive("(02) 1245 7896", true);
            PhoneContainerPositive("1300 777 777", true);
            PhoneContainerPositive("0212457896", false);
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
                ProfileTypeCode = ProfileConstants.Profiletype
            };

            await ClassUnderTest.ValidateAsync(invalidProfile);
        }

        /// <summary>
        /// Insert a profile record and check if the email has been updated .
        /// </summary>
        [TestMethod]
        public void ThrowsValidationExceptionIfUSIIsNull()
        {
            ChangeException(ValidationExceptionType.InvalidUSI);


            validProfile.USIs = new List<ApprenticeUSI>() {new ApprenticeUSI() {USI = "", ActiveFlag = true, USIStatus = "test"}};

            Container.GetMock<IUSIValidator>()
                .Setup(r => r.Validate(validProfile))
                .Returns(false);

            // ExecuteTest(validProfile);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Equals(false);
        }

        #region CRN

        [TestMethod]
        public void ThrowExceptionWhenCRNIsNull()
        {
            ChangeException(ValidationExceptionType.InvalidCRN);

            ClassUnderTest
                .Invoking(c => c.ValidateCRN(validProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void DoNothingIfCRNIsNotNUll()
        {
            // ChangeException(ValidationExceptionType.InvalidCRN);
            validProfile.CustomerReferenceNumber = ProfileConstants.CustomerReferenceNumber;
            ClassUnderTest
                .Invoking(c => c.ValidateCRN(validProfile))
                .Should().NotThrow();
        }

        #endregion
    }

    #endregion
}