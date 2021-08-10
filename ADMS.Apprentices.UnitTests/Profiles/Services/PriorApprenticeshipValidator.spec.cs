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
    public class WhenValidatingAPriorApprenticeship : GivenWhenThen<PriorApprenticeshipValidator>
    {
        private PriorApprenticeship priorApprenticeship;
        private Profile profile;

        private ValidationExceptionBuilder exceptionBuilder;

        protected override void Given()
        {
            profile = new Profile();
            priorApprenticeship = new PriorApprenticeship()
            {
                QualificationCode = "QCode",
                QualificationDescription = "QDescription",
                QualificationLevel = "524",
                QualificationANZSCOCode = "ANZS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1)
            };
            profile.PriorApprenticeships.Add(priorApprenticeship);
            profile.BirthDate = ProfileConstants.Birthdate;

            Container.GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidateAsync(It.IsAny<IQualificationAttributes>()))
                .ReturnsAsync(() => new ValidationExceptionBuilder());
        }

        protected override async void When()
        {
            exceptionBuilder = await ClassUnderTest.ValidateAsync(priorApprenticeship, profile);
        }

        [TestMethod]
        public void NoExceptionIfStartDateIsNull()
        {
            profile.PriorApprenticeships.Clear();
            priorApprenticeship.StartDate = null;
            profile.PriorApprenticeships.Add(priorApprenticeship);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(priorApprenticeship, profile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void NoExceptionIfEndDateIsNull()
        {
            profile.PriorApprenticeships.Clear();
            priorApprenticeship.EndDate = null;
            profile.PriorApprenticeships.Add(priorApprenticeship);
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(priorApprenticeship, profile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void NotThrowExceptionIfStartDateEndDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.PriorQualifications.Clear();
            priorApprenticeship.StartDate = ProfileConstants.Birthdate.AddYears(13);
            priorApprenticeship.EndDate = ProfileConstants.Birthdate.AddYears(14);
            profile.PriorApprenticeships.Add(priorApprenticeship);

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(priorApprenticeship, profile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void NotThrowExceptionIfEndDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.PriorApprenticeships.Clear();
            priorApprenticeship.EndDate = ProfileConstants.Birthdate.AddYears(14);
            profile.PriorApprenticeships.Add(priorApprenticeship);

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(priorApprenticeship, profile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }
    }

    #endregion
}