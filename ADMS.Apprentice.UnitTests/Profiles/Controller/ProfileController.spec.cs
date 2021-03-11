using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ADMS.Apprentice.Api.Controllers;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenCreatingAProfile

    [TestClass]
    public class WhenCreatingAProfileUsingApi : GivenWhenThen<ApprenticeProfileController>
    {
        private Profile profile;
        private ActionResult<ProfileModel> profileResult;
        private ProfileMessage message;


        private ProfileMessage CreateNewProfileMessage(string surName,
            String firstName,
            DateTime dob,
            String email = null,
            string ProfileType = null)
        {
            return new ProfileMessage
            {
                Surname = surName,
                FirstName = firstName,
                BirthDate = dob,
                EmailAddress = email,
                ProfileType = ProfileType
            };
        }

        protected override void Given()
        {
             
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, ProfileConstants.Birthdate, ProfileConstants.Emailaddress, ProfileConstants.Profiletype);
            profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                BirthDate = message.BirthDate,
                EmailAddress = message.EmailAddress,
                ProfileTypeCode  = message.ProfileType
            };

            Container
                .GetMock<IProfileCreator>()
                .Setup(r => r.CreateAsync(message))
                .Returns(Task.FromResult(profile));
        }

        protected override async void When()
        {
            profileResult = await ClassUnderTest.Create(message);
        }

        [TestMethod]
        public void ShouldReturnResult()
        {
            profileResult.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfNameNotValid()
        {
 
            message  = CreateNewProfileMessage("Bob$", ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Surname must contain only letters, spaces, hyphens and apostrophies");
        }


        [TestMethod]
        public void ShouldReturnNoValidationErrorIfNameIsValid()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25), null, ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        #region EmailAddressTests

        [TestMethod]
        public void ShouldReturnNoValidationErrorIfEmailIsNull()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25),null, ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfEmailIsInvalid()
        {
            

            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25),"test", ProfileConstants.Profiletype);
          //  var proptype = Enum.IsDefined(typeof(ProfileType), message.ProfileType) ? message.ProfileType : null;
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Invalid Email Address");
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfEmailLenghtExceedsMax()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, 
                ProfileConstants.Birthdate,ProfileConstants.Emailaddressmax256 + ProfileConstants.RandomString(100), ProfileConstants.Profiletype);
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Email Address Exceeds 256 Characters");
        }
    }

    #endregion

    #endregion
}