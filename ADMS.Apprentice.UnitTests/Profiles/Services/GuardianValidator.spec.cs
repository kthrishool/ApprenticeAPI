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

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    [TestClass]
    public class WhenValidatingGuardian : GivenWhenThen<GuardianValidator>
    {
        private Profile newProfile;
        private Guardian guardian;
        private ValidationException validationException;

        protected override void Given()
        {
            newProfile = new Profile();
            guardian = new Guardian();

            validationException = new ValidationException(null, (ValidationError) null);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.AddressRecordNotFoundForGuardian))
                .Returns(validationException);
        }

        protected override void When()
        {
            ClassUnderTest.ValidateAsync(guardian);
        }

        [TestMethod]
        public void DoNothingWhenAddressIsPassed()
        {
            guardian = new Guardian();
            guardian.StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1;
            guardian.StreetAddress2 = ProfileConstants.ResidentialAddress.StreetAddress2;
            guardian.StreetAddress3 = ProfileConstants.ResidentialAddress.StreetAddress3;
            guardian.Locality = ProfileConstants.ResidentialAddress.Locality;
            guardian.Postcode = ProfileConstants.ResidentialAddress.Postcode;
            guardian.StateCode = ProfileConstants.ResidentialAddress.StateCode;
            guardian.SingleLineAddress = ProfileConstants.ResidentialAddress.SingleLineAddress;
            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().NotThrow();
        }

        [TestMethod]
        public void DoNothingWhenSingleLineAddressIsPassed()
        {
            guardian = new Guardian();
            guardian.SingleLineAddress = "14 mort street, canberra center act 2601";
            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().NotThrow();
        }

        [TestMethod]
        public void throwExceptionWhenAddressIsIncomplete()
        {
            guardian.StreetAddress1 = "14 mort ";
            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void DoNothingIfEmailIsValid()
        {
            guardian.EmailAddress = ProfileConstants.Emailaddress;
            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void DONothingIfAddressIsEmpty()
        {
            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void ThrowExceptionWhenEmailIdInvalid()
        {
            validationException = new ValidationException(null, (ValidationError) null);

            Container.GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidEmailAddress))
                .Returns(validationException);
            guardian.EmailAddress = "test";
            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().Throw<ValidationException>();
        }
    }
}