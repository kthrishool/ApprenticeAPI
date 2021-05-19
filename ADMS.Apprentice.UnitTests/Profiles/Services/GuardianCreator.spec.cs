using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenCreatingAGuardian

    [TestClass]
    public class WhenCreatingAGuardian : GivenWhenThen<GuardianCreator>
    {
        private Guardian guardian;
        private ProfileGuardianMessage guardianMessage;
        private Profile profile;

        //  private Guard message;
        protected override void Given()
        {
            profile = new Profile();
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
            
        }

        protected override void When()
        {
            guardian = ClassUnderTest.CreateAsync(profile.Id, guardianMessage);
        }

        [TestMethod]
        public void DONothingWhenGuardianIsValid()
        {
            ClassUnderTest.Invoking(c => c.CreateAsync(profile.Id, guardianMessage))
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void ShouldSetValues()
        {
            guardian.ApprenticeId.Should().Be(profile.Id);
            guardian.FirstName.Should().Be(guardianMessage.FirstName);
            guardian.Surname.Should().Be(guardianMessage.Surname);
            //  guardian.ContactTypeCode.Should().Be(guardianMessage.ContactType);
            guardian.EmailAddress.Should().Be(guardianMessage.EmailAddress);
            guardian.StreetAddress1.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress1);
            guardian.StreetAddress2.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress2);
            guardian.Locality.Should().Be(ProfileConstants.ResidentialAddress.Locality);
            guardian.Postcode.Should().Be(ProfileConstants.ResidentialAddress.Postcode);
            guardian.StateCode.Should().Be(ProfileConstants.ResidentialAddress.StateCode);
        }
    }

    #endregion
}