using System;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                ProfileTypeCode = ProfileConstants.Profiletype
            };
            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = DateTime.Now.AddYears(-10),
                EmailAddress = ProfileConstants.RandomString(64) + "@" + ProfileConstants.RandomString(256) + "." + ProfileConstants.RandomString(50),
                ProfileTypeCode = ProfileConstants.Profiletype
            };

            validationException = new ValidationException(null, (ValidationError) null);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidApprenticeAge))
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
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(invalidProfile))
                .Should().Throw<ValidationException>();
        }

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
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidEmailAddress))
                .Returns(validationException);

            validProfile.EmailAddress = EmailAddress;
            //  await ClassUnderTest.ValidateAsync(invalidProfile);
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

            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(invalidProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        /// <summary>
        /// check if the ProfileType Is invalid
        /// </summary>
        [TestMethod]
        public void ThrowsValidationExceptionIfProfileTypeIsInvalid()
        {
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidApprenticeprofileType))
                .Returns(validationException);


            invalidProfile = new Profile
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = DateTime.Now.AddYears(-10),
                EmailAddress = ProfileConstants.RandomString(64) + "@" + ProfileConstants.RandomString(256) + "." + ProfileConstants.RandomString(50),
                ProfileTypeCode = "app"
            };


            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(invalidProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void DoesNothingIfForPhonePositiveTesting()
        {
            // Phone Number is null
            var phones = new Phone() {PhoneTypeCode = PhoneType.LandLine.ToString(), PhoneNumber = "0212345678"};

            validProfile.Phones = null; //.Add(phones);
            ClassUnderTest.ValidateAsync(validProfile);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfThePhoneNumberIsValid()
        {
            var phones = new Phone() {PhoneTypeCode = PhoneType.LandLine.ToString(), PhoneNumber = "0212345678"};

            validProfile.Phones.Add(phones);
            ClassUnderTest.ValidateAsync(validProfile);
        }

        [TestMethod]
        public void DoesNothingIfThePhoneNumberIsNull()
        {
            var phones = new Phone() {PhoneTypeCode = PhoneType.LandLine.ToString(), PhoneNumber = "0212345678"};

            validProfile.Phones.Add(phones);
            ClassUnderTest.ValidateAsync(validProfile);
        }

        private void PhoneContainerError(string phones)
        {
            var number = new Phone() {PhoneTypeCode = PhoneType.LandLine.ToString(), PhoneNumber = phones};
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidPhoneNumber))
                .Returns(validationException);

            validProfile.Phones.Add(number);
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(validProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        private void PhoneContainerPositive(string phones, Boolean needsConversion)
        {
            var number = new Phone() {PhoneTypeCode = PhoneType.LandLine.ToString(), PhoneNumber = phones};

            validProfile.Phones.Add(number);
            ClassUnderTest.ValidateAsync(validProfile);
            if (needsConversion)
                validProfile.Phones.Select(c => c.PhoneNumber).Should().NotContain(phones);
            else
                validProfile.Phones.Select(c => c.PhoneNumber).Should().Contain(phones);
        }

        [TestMethod]
        public void ThrowExceptonforInValidPhoneNumbers()
        {
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
    }

    #endregion
}