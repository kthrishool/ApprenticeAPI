using System;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using Adms.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentice.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentice.Core.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ADMS.Apprentice.UnitTests.Constants;

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
            String email = null)
        {
            return new ProfileMessage
            {
                Surname = surName,
                FirstName = firstName,
                BirthDate = dob,
                EmailAddress = email
            };
        }

        protected override void Given()
        {
             
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, ProfileConstants.Birthdate, ProfileConstants.Emailaddress);
            profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                BirthDate = message.BirthDate,
                EmailAddress = message.EmailAddress
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
 
            message  = CreateNewProfileMessage("Bob$", ProfileConstants.Firstname, DateTime.Now.AddYears(-25));
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Surname must contain only letters, spaces, hyphens and apostrophies");            
        }

       

        [TestMethod]
        public void ShouldReturnNoValidationErrorIfNameIsValid()
        {            
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
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25));
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(0);
        }
        [TestMethod]
        public void ShouldReturnValidationErrorIfEmailIsInvalid()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, DateTime.Now.AddYears(-25),"test");
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Invalid Email Address");
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfEmailLenghtExceedsMax()
        {
            message = CreateNewProfileMessage(ProfileConstants.Surname, ProfileConstants.Firstname, 
                ProfileConstants.Birthdate,ProfileConstants.Emailaddressmax256 + ProfileConstants.RandomString(100));
            var lstErrors = ValidateModel(message);
            lstErrors.Should().HaveCount(1);
            lstErrors[0].ErrorMessage.Should().StartWith("Email Address Exceeds 256 Characters");
        }

    }
#endregion
   

#endregion
}