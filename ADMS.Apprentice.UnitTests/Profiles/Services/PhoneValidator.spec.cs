using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adms.Shared.Testing;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.Core.Entities;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.Exceptions;
using FluentAssertions;

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
            validationException = new ValidationException(null, (ValidationError)null);

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
            validationException = new ValidationException(null, (ValidationError)null);

            Container
               .GetMock<IExceptionFactory>()
               .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidPhoneNumber))
               .Returns(validationException);

        }

        protected override void When()
        {
            ClassUnderTest.ValidatePhone(phone.PhoneNumber);
        }

        [TestMethod]
        public void DoesNothingIfTheAddressIsValid()
        {
            ClassUnderTest
                .Invoking(c => c.ValidatePhone(phone.PhoneNumber))
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
                .Invoking(c => c.ValidatePhone("+61 411111111"))
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
                .Invoking(c => c.ValidatePhone("+61 41111111"))
               .Should().Throw<ValidationException>();
        }
        [TestMethod]
        public void DoNothingWhenPhoneisNull()
        {
            ClassUnderTest
                 .Invoking(c => c.ValidatePhone(""))
                .Should().NotThrow<ValidationException>();
        }
    }

    #endregion 

}
