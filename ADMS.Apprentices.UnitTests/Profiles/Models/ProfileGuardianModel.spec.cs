using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Models
{
    [TestClass]
    public class WhenInstantiatingAProfileGuardianModel : GivenWhenThen
    {
        private ProfileGuardianModel profileGuardianModel;
        private Guardian guardian;

        protected override void Given()
        {
            guardian = new Guardian()
            {
                FirstName = ProfileConstants.Firstname,
                Surname = ProfileConstants.Surname,
                EmailAddress = ProfileConstants.Emailaddress,
                StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
                StreetAddress2 = ProfileConstants.ResidentialAddress.StreetAddress2,
                Locality = ProfileConstants.ResidentialAddress.Locality,
                StateCode = ProfileConstants.ResidentialAddress.StateCode,
                Postcode = ProfileConstants.ResidentialAddress.Postcode,
                SingleLineAddress = ProfileConstants.ResidentialAddress.SingleLineAddress,
                HomePhoneNumber = ProfileConstants.PhoneNumbers[0].PhoneNumber,
                Mobile = ProfileConstants.PhoneNumbers[0].PhoneNumber,
                WorkPhoneNumber = ProfileConstants.PhoneNumbers[0].PhoneNumber,
                ApprenticeId = 1,
                Id = 1
            };
        }

        protected override void When()
        {
            profileGuardianModel = new ProfileGuardianModel(guardian);
        }

        [TestMethod]
        public void ShouldSetPropertyOnModel()
        {
            profileGuardianModel.FirstName.Should().Be(ProfileConstants.Firstname);
            profileGuardianModel.Surname.Should().Be(ProfileConstants.Surname);
            profileGuardianModel.EmailAddress.Should().Be(ProfileConstants.Emailaddress);
            profileGuardianModel.Address.StreetAddress1.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress1);
            profileGuardianModel.Address.StreetAddress2.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress2);
            profileGuardianModel.Address.Locality.Should().Be(ProfileConstants.ResidentialAddress.Locality);
            profileGuardianModel.Address.StateCode.Should().Be(ProfileConstants.ResidentialAddress.StateCode);
            profileGuardianModel.Address.Postcode.Should().Be(ProfileConstants.ResidentialAddress.Postcode);
            profileGuardianModel.Address.SingleLineAddress.Should().Be(ProfileConstants.ResidentialAddress.SingleLineAddress);
            profileGuardianModel.HomePhoneNumber.Should().Be(ProfileConstants.PhoneNumbers[0].PhoneNumber);
            profileGuardianModel.Mobile.Should().Be(ProfileConstants.PhoneNumbers[0].PhoneNumber);
            profileGuardianModel.WorkPhoneNumber.Should().Be(ProfileConstants.PhoneNumbers[0].PhoneNumber);
            profileGuardianModel.ApprenticeId.Should().Be(1);
            profileGuardianModel.Id.Should().Be(1);
        }
    }
}