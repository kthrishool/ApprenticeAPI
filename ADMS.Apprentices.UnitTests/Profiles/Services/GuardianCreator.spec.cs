using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentices.Core.Services.Validators;
using Moq;
using Adms.Shared.Exceptions;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared;
using System.Threading.Tasks;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenCreatingAGuardian

    [TestClass]
    public class WhenCreatingAGuardian : GivenWhenThen<GuardianCreator>
    {        
        private Guardian guardian;
        private ProfileGuardianMessage guardianMessage;
        private Profile profile;
        private ValidationException validationException;

        //  private Guard message;
        protected override void Given()
        {
            profile = new Profile { Id = 1};
            guardian = new Guardian();
            guardianMessage = new ProfileGuardianMessage()
            {
                FirstName = ProfileConstants.Firstname,
                Surname = ProfileConstants.Surname,
                EmailAddress = ProfileConstants.Emailaddress,
                Address = ProfileConstants.ResidentialAddress,
                Mobile = "0411111111",
                HomePhoneNumber = "0211111111",
                WorkPhoneNumber = "0411111111"
            };

            Container.GetMock<IGuardianValidator>()
                .Setup(s => s.ValidateAsync(It.IsAny<Guardian>()))
                .ReturnsAsync(new ValidationExceptionBuilder(Container.GetMock<IExceptionFactory>().Object));

            validationException = new ValidationException(null, (ValidationError)null);
            Container
               .GetMock<IRepository>()
               .Setup(r => r.GetAsync<Profile>(profile.Id, true))
               .Returns(Task.FromResult(profile));
        }

        protected override async void When()
        {
            guardian = await ClassUnderTest.CreateAsync(profile.Id, guardianMessage);
        }

        [TestMethod]
        public void DONothingWhenGuardianIsValid()
        {
            ClassUnderTest.Invoking(async c => await c.CreateAsync(profile.Id, guardianMessage))
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void ShouldSetValues()
        {
            guardian.ApprenticeId.Should().Be(profile.Id);
            guardian.FirstName.Should().Be(guardianMessage.FirstName);
            guardian.Surname.Should().Be(guardianMessage.Surname);
            guardian.EmailAddress.Should().Be(guardianMessage.EmailAddress);
            guardian.StreetAddress1.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress1);
            guardian.StreetAddress2.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress2);
            guardian.Locality.Should().Be(ProfileConstants.ResidentialAddress.Locality);
            guardian.Postcode.Should().Be(ProfileConstants.ResidentialAddress.Postcode);
            guardian.StateCode.Should().Be(ProfileConstants.ResidentialAddress.StateCode);
        }

        [TestMethod]
        public void ThrowExceptionIfGuardianExists()
        {
            profile.Guardian = guardian;

            Container.GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.GuardianExists))
                .Returns(validationException);

            ClassUnderTest.Invoking(async c => await c.CreateAsync(profile.Id, guardianMessage)).Should().Throw<ValidationException>();
        }
    }

    #endregion
}