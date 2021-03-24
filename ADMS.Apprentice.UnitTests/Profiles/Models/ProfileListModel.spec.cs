using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Models
{
    #region WhenInstantiatingAProfileListModel

    [TestClass]
    public class WhenInstantiatingAProfileListModel : GivenWhenThen
    {
        private ProfileListModel model;
        private Profile profile;

        protected override void Given()
        {
            profile = new Profile
            {
                Id = 123,
                FirstName = ProfileConstants.Surname,
                Surname = ProfileConstants.Firstname,
                OtherNames = ProfileConstants.Secondname,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress,
                ProfileTypeCode = ProfileConstants.Profiletype,
                CreatedOn = ProfileConstants.Createdon,
                CreatedBy = ProfileConstants.Createdby,
                UpdatedOn = ProfileConstants.Updatedon,
                UpdatedBy = ProfileConstants.Updatedby
            };
        }

        protected override void When()
        {
            model = new ProfileListModel(profile);
        }

        [TestMethod]
        public void ReturnsAModel()
        {
            model.Should().NotBeNull();
        }

        /// <summary>
        /// Sets all the properties for the models.
        /// </summary>
        [TestMethod]
        public void SetsPropertiesOnModel()
        {
            model.Id.Should().Be(123);
            model.FirstName.Should().Be(ProfileConstants.Surname);
            model.Surname.Should().Be(ProfileConstants.Firstname);
            model.OtherNames.Should().Be(ProfileConstants.Secondname);
            model.BirthDate.Should().BeCloseTo(ProfileConstants.Birthdate);
            model.EmailAddress.Should().Be(ProfileConstants.Emailaddress);
            model.CreatedOn.Should().BeCloseTo(ProfileConstants.Createdon);
            model.UpdatedOn.Should().BeCloseTo(ProfileConstants.Updatedon);
            model.CreatedBy.Should().Be(ProfileConstants.Createdby);
            model.UpdatedBy.Should().Be(ProfileConstants.Updatedby);
            model.Phones = null;
            model.ResidentialAddress = null;
            model.PostalAddress = null;
        }
    }

    #endregion
}