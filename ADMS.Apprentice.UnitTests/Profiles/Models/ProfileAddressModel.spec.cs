using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Models
{
    #region WhenInstantiatingAProfileListModel

    [TestClass]
    public class WhenInstantiatingAProfileAddressModel : GivenWhenThen
    {        
        private ProfileAddressModel profile;

        protected override void Given()
        {
            profile = new ProfileAddressModel()
            {
                StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
                StreetAddress2 = ProfileConstants.ResidentialAddress.StreetAddress2,
                StreetAddress3 = ProfileConstants.ResidentialAddress.StreetAddress3,
                Locality = ProfileConstants.ResidentialAddress.Locality,
                Postcode = ProfileConstants.ResidentialAddress.Postcode,
                StateCode = ProfileConstants.ResidentialAddress.StateCode,
                SingleLineAddress = ProfileConstants.ResidentialAddress.SingleLineAddress
            };
        }

        //protected override void When()
        //{
        //    model = new ProfileListModel(profile);
        //}

        //[TestMethod]
        //public void ReturnsAModel()
        //{
        //    model.Should().NotBeNull();
        //}

        /// <summary>
        /// Sets all the properties for the models.
        /// </summary>
        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            profile.StreetAddress1.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress1);
            profile.StreetAddress2.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress2);
            profile.StreetAddress3.Should().Be(ProfileConstants.ResidentialAddress.StreetAddress3);
            profile.Locality.Should().Be(ProfileConstants.ResidentialAddress.Locality);
            profile.Postcode.Should().Be(ProfileConstants.ResidentialAddress.Postcode);
            profile.StateCode.Should().Be(ProfileConstants.ResidentialAddress.StateCode);
            profile.SingleLineAddress.Should().Be(ProfileConstants.ResidentialAddress.SingleLineAddress);
        }
    }

    #endregion
}