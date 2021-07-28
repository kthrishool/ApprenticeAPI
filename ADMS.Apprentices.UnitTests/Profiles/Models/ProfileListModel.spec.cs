using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Models
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
                PreferredName = ProfileConstants.PreferredName,
                GenderCode = ProfileConstants.GenderCode,
                InterpretorRequiredFlag = ProfileConstants.InterpretorRequiredFlag,
                LanguageCode = ProfileConstants.LanguageCode               
            };
            profile.USIs.Add(new ApprenticeUSI { USI = "usi", ActiveFlag = true });
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
            model.ProfileType.Should().Be(ProfileType.APPR.ToString());            
        }

        [TestMethod]
        public void SetUSINullIfNoActiveUSI()
        {
            //given
            profile.USIs.Clear();
            profile.USIs.Add(new ApprenticeUSI { USI = "usi", ActiveFlag = false });

            //when
            model = new ProfileListModel(profile);

            //then
            model.USI.Should().BeNull();
        }
    }

    #endregion

    #region WhenInstantiatingAProfileListModel

    [TestClass]
    public class WhenInstantiatingAProfileListModelFromSearchResults : GivenWhenThen
    {
        private ProfileListModel model;
        private ProfileSearchResultModel profile;

        protected override void Given()
        {
            profile = new ProfileSearchResultModel(
                123, ProfileConstants.Profiletype, ProfileConstants.Firstname,
                ProfileConstants.Surname, ProfileConstants.Secondname,
                ProfileConstants.Birthdate, ProfileConstants.Emailaddress,
                ProfileConstants.USI, null, null, null, 20);
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
            model.FirstName.Should().Be(ProfileConstants.Firstname);
            model.Surname.Should().Be(ProfileConstants.Surname);
            model.OtherNames.Should().Be(ProfileConstants.Secondname);
            model.BirthDate.Should().BeCloseTo(ProfileConstants.Birthdate);
            model.EmailAddress.Should().Be(ProfileConstants.Emailaddress);
            model.ProfileType.Should().Be(ProfileType.APPR.ToString());
        }
    }
       
    #endregion
}