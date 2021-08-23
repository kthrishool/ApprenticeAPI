using System;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.Core.TYIMS.Entities;
using ADMS.Apprentices.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingAPriorQualification

    [TestClass]
    public class WhenValidatingAPriorQualification : GivenWhenThen<PriorQualificationValidator>
    {
        private PriorQualification qualification;
        private Profile profile;

        private ValidationExceptionBuilder exceptionBuilder;

        protected override void Given()
        {
            profile = new Profile();
            qualification = new PriorQualification()
            {
                QualificationCode = "QCode",
                QualificationDescription = "QDescription",
                QualificationLevel = "524",
                QualificationANZSCOCode = "ANZS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1)
            };
            profile.PriorQualifications.Add(qualification);
            profile.BirthDate = ProfileConstants.Birthdate;

            Container.GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorQualificationsAsync(It.IsAny<PriorQualification>()))
                .ReturnsAsync(() => new ValidationExceptionBuilder());
        }


        protected override async void When()
        {
            exceptionBuilder = await ClassUnderTest.ValidatePriorQualificationAsync(qualification, profile);
        }


        [TestMethod]
        public void NoExceptionIfStartDateIsNull()
        {
            profile.PriorQualifications.Clear();
            qualification.StartDate = null;
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void NoExceptionIfEndDateIsNull()
        {
            profile.PriorQualifications.Clear();
            qualification.EndDate = null;
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void NoExceptionIfStartAndEndDateIsNull()
        {
            profile.PriorQualifications.Clear();
            qualification.EndDate = null;
            qualification.StartDate = null;
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfFoundDuplicate()
        {
            profile.PriorQualifications.Clear();
            profile.PriorQualifications.Add(qualification);
            profile.PriorQualifications.Add(qualification);

            ClassUnderTest.Invoking(c => c.CheckForDuplicates(profile.PriorQualifications.ToList()).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void DoesNotThrowValidationExceptionIfNoDuplicate()
        {
            profile.PriorQualifications.Clear();
            profile.PriorQualifications.Add(qualification);

            ClassUnderTest.Invoking(c => c.CheckForDuplicates(profile.PriorQualifications.ToList()).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsLessThanStartDate()
        {
            profile.PriorQualifications.Clear();
            qualification.StartDate = new DateTime(2020, 1, 2);
            qualification.EndDate = new DateTime(2020, 1, 1);
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }


        [TestMethod]
        public void ThrowsExceptionIfStartDateIsGreaterThanTodaysDate()
        {
            profile.PriorQualifications.Clear();
            qualification.StartDate = DateTime.Now.AddDays(+1);
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsGraterThanTodaysDate()
        {
            profile.PriorQualifications.Clear();
            qualification.EndDate = DateTime.Now.AddDays(+1);
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfStartDateEndDateIsGraterThanTodaysDate()
        {
            profile.PriorQualifications.Clear();
            qualification.StartDate = DateTime.Now.AddDays(+1);
            qualification.EndDate = DateTime.Now.AddDays(+1);
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfStartDateIsLessThanDateofBirthPlus12Years()
        {
            profile.PriorQualifications.Clear();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(11);
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfEndDateIsLessThanDateofBirthPlus12Years()
        {
            profile.PriorQualifications.Clear();
            qualification.StartDate = null;
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(11);
            profile.PriorQualifications.Add(qualification);
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfStartDateEndDateIsLessThanDateofBirthPlus12Years()
        {
            profile.PriorQualifications.Clear();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(10);
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(11);

            profile.PriorQualifications.Add(qualification);
            var b = ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)));
            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void NotThrowExceptionIfStartDateEndDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.PriorQualifications.Clear();
            qualification.StartDate = ProfileConstants.Birthdate.AddYears(13);
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(14);
            profile.PriorQualifications.Add(qualification);

            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void NotThrowExceptionIfEndDateIsGreaterThanDateofBirthPlus12Years()
        {
            profile.PriorQualifications.Clear();
            qualification.EndDate = ProfileConstants.Birthdate.AddYears(14);
            profile.PriorQualifications.Add(qualification);

            ClassUnderTest.Invoking(async c => (await c.ValidatePriorQualificationAsync(qualification, profile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }
    }

    #endregion

    #region "Qualification Validation with Apprenticeship"

    [TestClass]
    public class WhenValidatingAQualificationWithAnApprenticeship : GivenWhenThen<PriorQualificationValidator>
    {
        private ValidationException validationException;
        private PriorQualification qualification;
        private Profile profile;
        private Registration registration;

        protected override void Given()
        {
            profile = new Profile();
            qualification = new PriorQualification()
            {
                QualificationCode = "QCode",
                QualificationDescription = "QDescription",
                QualificationLevel = "524",
                QualificationANZSCOCode = "ANZS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1),
                ApprenticeshipId = 20,
                Id = ProfileConstants.Id
            };
            profile.PriorQualifications.Add(qualification);
            profile.BirthDate = ProfileConstants.Birthdate;
            validationException = new ValidationException(null, (ValidationError) null);

            registration = new Registration()
            {
                CurrentEndReasonCode = "CMPS",
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2020, 1, 1),
                RegistrationId = qualification.ApprenticeshipId.Value,
                QualificationCode = "QCode",
                TrainingContractId = 100,
                ClientId = ProfileConstants.Id
            };

            Container.GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorQualificationsAsync(It.IsAny<PriorQualification>()))
                .ReturnsAsync(() => new ValidationExceptionBuilder());
        }
    }

    #endregion "Qualification Validation with Apprenticeship"
}