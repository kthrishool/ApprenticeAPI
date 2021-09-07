using System;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingAPriorApprenticeship

    [TestClass]
    public class WhenValidatingAPriorApprenticeship : GivenWhenThen<PriorApprenticeshipQualificationValidator>
    {
        private PriorApprenticeshipQualification priorApprenticeship;
        private Profile profile;

        protected override void Given()
        {
            profile = new Profile();
            priorApprenticeship = new PriorApprenticeshipQualification
            {
                QualificationCode = "QCode",
                QualificationDescription = "QDescription",
                QualificationLevel = "524",
                QualificationANZSCOCode = "ANZS",
                StartDate = new DateTime(2010, 1, 1),
                ApprenticeshipReference = "apprenticeship-reference"
            };
            profile.PriorApprenticeshipQualifications.Add(priorApprenticeship);
            profile.BirthDate = ProfileConstants.Birthdate;

        }

        protected override void When()
        {
            ClassUnderTest.Validate(priorApprenticeship, profile);
        }

        [TestMethod]
        public void NoExceptionIfStartDateIsNull()
        {
            profile.PriorApprenticeshipQualifications.Clear();
            priorApprenticeship.StartDate = null;
            profile.PriorApprenticeshipQualifications.Add(priorApprenticeship);
            ClassUnderTest.Invoking(c => (c.Validate(priorApprenticeship, profile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void DoesNotThrowExceptionIfStartDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.PriorQualifications.Clear();
            priorApprenticeship.StartDate = ProfileConstants.Birthdate.AddYears(13);
            profile.PriorApprenticeshipQualifications.Add(priorApprenticeship);

            ClassUnderTest.Invoking(c => (c.Validate(priorApprenticeship, profile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }
    }

    #endregion
}