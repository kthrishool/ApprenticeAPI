using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentice.Core.Services.Validators;
using System.Threading.Tasks;
using Adms.Shared.Exceptions;
using Moq;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenUpdatingAGuardian

    [TestClass]
    public class WhenUpdatingAGuardian : GivenWhenThen<GuardianUpdater>
    {
        private Guardian guardian;
        private ProfileGuardianMessage guardianMessage;

        //  private Guard message;
        protected override void Given()
        {
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
                .Setup(s => s.ValidateAsync(guardian))
                .ReturnsAsync(new ValidationExceptionBuilder(Container.GetMock<IExceptionFactory>().Object));
        }

        protected override async void When()
        {
            guardian = await ClassUnderTest.Update(guardian, guardianMessage);
        }

        [TestMethod]
        public void ShouldValidatesTheRequest()
        {
            Container.GetMock<IGuardianValidator>().Verify(r => r.ValidateAsync(guardian));
        }

        [TestMethod]
        public void ShouldSetValues()
        {
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
        public async Task ShouldSetAddressAttributesToNullIfNoAddress()
        {
            //given
            guardianMessage = new ProfileGuardianMessage()
            {
                FirstName = ProfileConstants.Firstname,
                Surname = ProfileConstants.Surname,
                EmailAddress = ProfileConstants.Emailaddress,
                Address = null,
                Mobile = "0411111111",
                HomePhoneNumber = "0211111111",
                WorkPhoneNumber = "0411111111"
            };

            //when
            guardian = await ClassUnderTest.Update(guardian, guardianMessage);

            guardian.StreetAddress1.Should().Be(null);
            guardian.StreetAddress2.Should().Be(null);
            guardian.Locality.Should().Be(null);
            guardian.Postcode.Should().Be(null);
            guardian.StateCode.Should().Be(null);
        }
    }

    #endregion
}