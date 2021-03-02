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

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenCreatingAProfile
    [TestClass]
    public class WhenCreatingAProfileUsingApi : GivenWhenThen<ApprenticeProfileController>
    {
        private Profile profile;
        private ActionResult<ProfileModel> profileResult;
        private ProfileMessage message;       
        protected override void Given()
        {
            message = new ProfileMessage
            {
                Surname = "Bob",
                FirstName = "Alex",
                BirthDate = DateTime.Now.AddYears(-25)
            };

            profile = new Profile
            {
                Surname = message.Surname,
                FirstName = message.FirstName,
                BirthDate = message.BirthDate,
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
            message = new ProfileMessage
            {
                Surname = "Bob$",
                FirstName = "Alex",
                BirthDate = DateTime.Now.AddYears(-25)
            };
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

    }

    #endregion
}