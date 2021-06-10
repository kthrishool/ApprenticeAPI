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
using System.Threading.Tasks;
using Adms.Shared;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    [TestClass]
    public class WhenValidatingGuardian : GivenWhenThen<GuardianValidator>
    {
        private Profile profile;
        private Guardian guardian;
        private ValidationException validationException;

        protected override void Given()
        {
            profile = new Profile
            {          
                Id = 1,
                Surname = ProfileConstants.Surname,
                BirthDate = ProfileConstants.Birthdate,
                Guardian = null
            };
            guardian = new Guardian { Id = 1, ApprenticeId = 1 };

            validationException = new ValidationException(null, (ValidationError) null);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.AddressRecordNotFoundForGuardian))
                .Returns(validationException);

            Container
               .GetMock<IRepository>()
               .Setup(r => r.GetAsync<Profile>(profile.Id, true))
               .Returns(Task.FromResult(profile));
        }

        protected override void When()
        {
            ClassUnderTest.ValidateAsync(guardian);
        }

        [TestMethod]
        public void DoNothingWhenAddressIsPassed()
        {
            guardian = new Guardian();
            guardian.ApprenticeId = 1;
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
            guardian.ApprenticeId = 1;
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

            Container.GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidEmailAddress))
                .Returns(validationException);
            guardian.EmailAddress = "test";
            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowExceptionIfGuardianExists()
        {
            profile.Guardian = guardian;

            Container.GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.GuardianExists))
                .Returns(validationException);

            ClassUnderTest.Invoking(c => c.ValidateAsync(guardian)).Should().Throw<ValidationException>();
        }
    }
}