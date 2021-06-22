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
        
        private IValidatorExceptionBuilder exceptionBuilder;

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
            exceptionBuilder = new ValidatorExceptionBuilder(Container.GetMock<IExceptionFactory>().Object);

            Container.GetMock<IValidatorExceptionBuilderFactory>()
                .Setup(f => f.CreateExceptionBuilder())
                .Returns(exceptionBuilder);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidPhoneNumber))
                .Returns(validationException);
        }

        protected override void When()
        {
            ClassUnderTest.ValidatePhonewithType(exceptionBuilder, phone);
        }

        [TestMethod]
        public void DoesNothingIfTheAddressIsValid()
        {
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(exceptionBuilder, phone)).Invoke();
            exceptionBuilder.HasExceptions()
                .Should().Be(false);
        }

        [TestMethod]
        public void FormatPhoneWithInvalidChars()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 411111111"
            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(exceptionBuilder, phone)).Invoke();

            exceptionBuilder.HasExceptions()
                .Should().Be(false);
        }

        [TestMethod]
        public void MobileNumberWithInvalidTypeThrowException()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 411111111",
                PhoneTypeCode = PhoneType.LANDLINE.ToString()

            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(exceptionBuilder, phone)).Invoke();

            exceptionBuilder.HasExceptions()
                .Should().Be(true);
        }

        [TestMethod]
        public void LandLineWithInvalidTypeThrowException()
        {
            phone = new Phone()
            {
                PhoneNumber = "0262400387",
                PhoneTypeCode = PhoneType.MOBILE.ToString()

            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(exceptionBuilder, phone)).Invoke();

            exceptionBuilder.HasExceptions()
                .Should().Be(true);
        }

        [TestMethod]
        public void SetPhoneTypeAsLandLine()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61262400387"
            };
            ClassUnderTest.ValidatePhonewithType(exceptionBuilder, phone);

            phone.PhoneTypeCode.Should().Be(PhoneType.LANDLINE.ToString());
        }

        [TestMethod]
        public void throwExceptionWhenpassingAInvalidNumber()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 41111111"
            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(exceptionBuilder, phone)).Invoke();

            exceptionBuilder.HasExceptions()
                .Should().Be(true);
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
        private IValidatorExceptionBuilder exceptionBuilder;

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
            exceptionBuilder = new ValidatorExceptionBuilder(Container.GetMock<IExceptionFactory>().Object);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidPhoneNumber))
                .Returns(validationException);
        }

        protected override void When()
        {
            ClassUnderTest.ValidatePhone(exceptionBuilder, phone.PhoneNumber, ValidationExceptionType.InvalidPhoneNumber);
        }

        [TestMethod]
        public void DoesNothingIfTheAddressIsValid()
        {
            ClassUnderTest
                .Invoking(c => c.ValidatePhone(exceptionBuilder, phone.PhoneNumber, ValidationExceptionType.InvalidPhoneNumber))
                .Invoke();
            exceptionBuilder.HasExceptions()
                .Should().Be(false);
        }

        [TestMethod]
        public void FormatPhoneWithInvalidChars()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 411111111"
            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhone(exceptionBuilder, "+61 411111111", ValidationExceptionType.InvalidPhoneNumber))
                .Invoke();

            exceptionBuilder.HasExceptions()
                .Should().Be(false);
        }

        [TestMethod]
        public void throwExceptionWhenpassingAInvalidNumber()
        {
            phone = new Phone()
            {
                PhoneNumber = "+61 41111111"
            };
            ClassUnderTest
                .Invoking(c => c.ValidatePhone(exceptionBuilder, "+61 41111111", ValidationExceptionType.InvalidPhoneNumber))
                .Invoke();

            exceptionBuilder.HasExceptions().Should().Be(true);
        }

        [TestMethod]
        public void WhenPhoneisNull()
        {
            ClassUnderTest
                .Invoking(c => c.ValidatePhone(exceptionBuilder, "", ValidationExceptionType.InvalidPhoneNumber))
                .Invoke();

            exceptionBuilder.HasExceptions().Should().Be(false);
        }

        [TestMethod]
        public void HaveExceptionWhenPhoneNumberisTooLong()
        {
            var phoneNumber = "+61 41234567891011";
            ClassUnderTest
                .Invoking(c => c.ValidatePhone(exceptionBuilder, phoneNumber, ValidationExceptionType.InvalidPhoneNumber))
                .Invoke();

            exceptionBuilder.HasExceptions().Should().Be(true);
        }

        [TestMethod]
        public void HaveExceptionWhenPhoneNmberAreaCodeIsInvalid()
        {
            var phoneNumber = "(09) 6121 8390";
            ClassUnderTest
                .Invoking(c => c.ValidatePhone(exceptionBuilder, phoneNumber, ValidationExceptionType.InvalidPhoneNumber))
                .Invoke();

            exceptionBuilder.HasExceptions().Should().Be(true);
        }

        [TestMethod]
        public void HaveExceptionWhenPhoneAreaCodeIsInvalid()
        {
            phone.PhoneNumber = "(09) 6121 8390";
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(exceptionBuilder, phone))
                .Invoke();

            exceptionBuilder.HasExceptions().Should().Be(true);
        }


        [TestMethod]
        public void HasExceptionWhenPhoneNumberIsEmpty()
        {
            phone.PhoneNumber = "";
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(exceptionBuilder, phone))
                .Invoke();
            exceptionBuilder.HasExceptions()
                .Should().Be(true);
        }

        [TestMethod]
        public void HaveExceptionWhenPhoneisTooLong()
        {
            phone.PhoneNumber = "+61 41234567891011";
            ClassUnderTest
                .Invoking(c => c.ValidatePhonewithType(exceptionBuilder, phone))
                .Invoke();

            exceptionBuilder.HasExceptions().Should().Be(true);
        }
    }

    #endregion
}