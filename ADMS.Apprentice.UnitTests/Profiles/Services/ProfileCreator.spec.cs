using System;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenCreatingAProfile
    [TestClass]
    public class WhenCreatingAProfile : GivenWhenThen<ProfileCreator>
    {        
        private Profile profile;
        private ProfileMessage message;

        protected override void Given()
        {
            message = new ProfileMessage
            {
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
                BirthDate = ProfileConstants.Birthdate,
                EmailAddress = ProfileConstants.Emailaddress

            };
        }

        protected override async void When()
        {
            profile = await ClassUnderTest.CreateAsync(message);
        }

        [TestMethod]
        public void ShouldReturnProfile()
        {
            profile.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldAddTheProfileToTheDatabase()
        {
            Container.GetMock<IRepository>().Verify(r => r.Insert(profile));
        }

        [TestMethod]
        public void ShouldValidatesTheProfileRequest()
        {
            Container.GetMock<IProfileValidator>().Verify(r => r.ValidateAsync(profile));
        }

        [TestMethod]
        public void ShouldSetTheName()
        {           
            profile.FirstName.Should().Be(message.FirstName);
            profile.Surname.Should().Be(message.Surname);
        }       

            [TestMethod]
        public void ShouldSetTheBirthDate()
        {
            profile.BirthDate.Should().Be(message.BirthDate);
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            profile.DeceasedFlag.Should().BeFalse();
            profile.ActiveFlag.Should().BeTrue();            
        }

        [TestMethod]
        public void ShouldSetProfileType()
        {
            profile.ProfileTypeCode.Should().Be(ProfileType.Apprentice.ToString());
        }

        /// <summary>
        /// Insert a profile record and check if the email has been updated .
        /// </summary>
        [TestMethod]
        public void CheckIfDataBasedHasBeenUpdated()
        {
            profile.EmailAddress.Should().Be(message.EmailAddress);
        }

    }

    #endregion
}