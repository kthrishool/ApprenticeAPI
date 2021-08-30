using System;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentices.Core.Entities;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingDeceasedDetailsOfTheProfile

    [TestClass]
    public class WhenValidatingDeceasedDetailsOfTheProfile : GivenWhenThen<DeceasedValidator>
    {
        private Profile profile;

        protected override void Given()
        {
            profile = new Profile
            {
                BirthDate = DateTime.Now.AddYears(-20).Date,
                DeceasedFlag = true,
                DeceasedDate = DateTime.Now.AddYears(-10).Date,
                ActiveFlag = false,
                InactiveDate = DateTime.Now.Date,
            };                
        }

        [TestMethod]
        public void DoesNothingIfAllValid()
        {
            ClassUnderTest.Validate(profile);
        }

        [TestMethod]
        public void ThrowsAnExceptionIfDeceasedDateIsNull()
        {
            profile.DeceasedDate = null;
            ClassUnderTest.Invoking(c => c.Validate(profile))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.DeceasedDateRequired));
        }

        [TestMethod]
        public void ThrowsAnExceptionIfDeceasedDateBeforeBirthDate()
        {
            profile = new Profile
            {
                BirthDate = DateTime.Now.AddDays(-12).Date,
                DeceasedFlag = true,
                DeceasedDate = DateTime.Now.Date.AddDays(-13).Date,
                ActiveFlag = false,
                InactiveDate = DateTime.Now.Date,
            };
            ClassUnderTest.Invoking(c => c.Validate(profile))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.DeceasedDateDOBMismatch));
        }

        [TestMethod]
        public void ThrowsAnExceptionIfDeceasedDateLaterThanCurrentDate()
        {
            profile = new Profile
            {
                BirthDate = DateTime.Now.AddDays(-12).Date,
                DeceasedFlag = true,
                DeceasedDate = DateTime.Now.Date.AddDays(1).Date,
                ActiveFlag = false,
                InactiveDate = DateTime.Now.Date,
            };
            ClassUnderTest.Invoking(c => c.Validate(profile))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.DeceasedDateCurrentDateMismatch));
        }

        [TestMethod]
        public void ThrowsAnExceptionIfNotDeceasedAndDeceasedDateSet()
        {
            profile = new Profile
            {
                BirthDate = DateTime.Now.AddDays(-12).Date,
                DeceasedFlag = false,
                DeceasedDate = DateTime.Now.Date.AddDays(-10).Date,
                ActiveFlag = false,
                InactiveDate = DateTime.Now.Date,
            };
            ClassUnderTest.Invoking(c => c.Validate(profile))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.DeceasedFlagDeceasedDateMismatch));
        }
    }

    #endregion
}