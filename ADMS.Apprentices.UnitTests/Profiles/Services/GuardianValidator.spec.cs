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
using System.Threading.Tasks;
using Adms.Shared;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
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
            Container
                .GetMock<IAddressValidator>()
                .Setup(av => av.ValidateAsync(It.IsAny<IAddressAttributes>()))
                .ReturnsAsync(new ValidationExceptionBuilder(Container.GetMock<IExceptionFactory>().Object));
        }

        protected override void When()
        {
            //ClassUnderTest.ValidateAsync(guardian);
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
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(guardian)).ThrowAnyExceptions()).Should().NotThrow();
        }

        [TestMethod]
        public void DoNothingWhenSingleLineAddressIsPassed()
        {
            guardian = new Guardian();
            guardian.ApprenticeId = 1;
            guardian.SingleLineAddress = "14 mort street, canberra center act 2601";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(guardian)).ThrowAnyExceptions()).Should().NotThrow();
        }

        [TestMethod]
        public void throwExceptionWhenAddressIsIncomplete()
        {            
            guardian.StreetAddress1 = "14 mort ";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(guardian)).ThrowAnyExceptions())
            .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void DoNothingIfEmailIsValid()
        {
            guardian.EmailAddress = ProfileConstants.Emailaddress;
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(guardian)).ThrowAnyExceptions()).Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void DONothingIfAddressIsEmpty()
        {
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(guardian)).ThrowAnyExceptions()).Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void ThrowExceptionWhenEmailIdInvalid()
        {

            Container.GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidEmailAddress))
                .Returns(validationException);
            guardian.EmailAddress = "test";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(guardian)).ThrowAnyExceptions()).Should().Throw<ValidationException>();
        }
    }
}