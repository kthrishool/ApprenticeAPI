using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenValidatingaPhoneNumberByPhoneEntity

    [TestClass]
    public class WhenValidatingaPhoneNumberByPhoneEntity : GivenWhenThen<PhoneValidator>
    {
        private Profile newProfile;
        private Phone phone;
        private ValidationException validationException;

        protected override void Given()
        {
            newProfile = new Profile();
            phone = new Phone()
            {
                PhoneNumber = "0411111111",
                PhoneTypeCode = "MOBILE",
                PreferredPhoneFlag = true
            };
            validationException = new ValidationException(null, (ValidationError) null);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidPhoneNumber))
                .Returns(validationException);
        }

        protected override void When()
        {
            ClassUnderTest.ValidatePhonewithType(phone);
        }

        [TestMethod]
        public void DoesNothingIfTheAddressIsValid()
        {
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(phone))
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void FormatPhoneWithInvalidChars()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 411111111"
            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(phone))
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void throwExceptionWhenpassingAInvalidNumber()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 41111111"
            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(phone))
                .Should().Throw<ValidationException>();
        }
    }

    #endregion


    #region WhenValidatingaPhoneNumberasString

    [TestClass]
    public class WhenValidatingaPhoneNumberasString : GivenWhenThen<PhoneValidator>
    {
        private Profile newProfile;
        private Phone phone;
        private ValidationException validationException;

        protected override void Given()
        {
            newProfile = new Profile();
            phone = new Phone()
            {
                PhoneNumber = "0411111111",
                PhoneTypeCode = "MOBILE",
                PreferredPhoneFlag = true
            };
            validationException = new ValidationException(null, (ValidationError) null);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidPhoneNumber))
                .Returns(validationException);
        }

        protected override void When()
        {
            ClassUnderTest.ValidatePhone(phone.PhoneNumber, ValidationExceptionType.InvalidPhoneNumber);
        }

        [TestMethod]
        public void DoesNothingIfTheAddressIsValid()
        {
            ClassUnderTest
                .Invoking(c => c.ValidatePhone(phone.PhoneNumber, ValidationExceptionType.InvalidPhoneNumber))
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void FormatPhoneWithInvalidChars()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 411111111"
            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhone("+61 411111111", ValidationExceptionType.InvalidPhoneNumber))
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void throwExceptionWhenpassingAInvalidNumber()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 41111111"
            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhone("+61 41111111", ValidationExceptionType.InvalidPhoneNumber))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void DoNothingWhenPhoneisNull()
        {
            ClassUnderTest
                .Invoking(c => c.ValidatePhone("", ValidationExceptionType.InvalidPhoneNumber))
                .Should().NotThrow<ValidationException>();
        }
    }

    #endregion
}