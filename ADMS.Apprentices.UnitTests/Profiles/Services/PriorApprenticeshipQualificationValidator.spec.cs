using System;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingAPriorApprenticeshipQualification

    [TestClass]
    public class WhenValidatingAPriorApprenticeshipQualification : GivenWhenThen<PriorApprenticeshipQualificationValidator>
    {
        private PriorApprenticeshipQualification valid;
        private PriorApprenticeshipQualification invalidTooYoung;
        private Profile apprentice;
        private PriorApprenticeshipQualification invalidFuture;
        private PriorApprenticeshipQualification invalidRefCodes;
        private PriorApprenticeshipQualification invalidMissingAnzsco;
        private PriorApprenticeshipQualification invalidMissingLevel;
        private PriorApprenticeshipQualification validManuallyEntered;
        private readonly DateTime validStartDate = new DateTime(2015, 3, 23);

        protected override void Given()
        {
            valid = new PriorApprenticeshipQualification {StartDate = validStartDate};
            invalidTooYoung = new PriorApprenticeshipQualification {StartDate = new DateTime(1991, 12, 31)};
            invalidFuture = new PriorApprenticeshipQualification {StartDate = new DateTime(2030, 1, 3)};
            invalidRefCodes = new PriorApprenticeshipQualification {StartDate = new DateTime(2015, 3, 2)};
            invalidMissingAnzsco = new PriorApprenticeshipQualification
            {
                StartDate = validStartDate,
                QualificationManualReasonCode = PriorApprenticeshipQualification.ManuallyEnteredCode,
                QualificationLevel = "level"
            };
            invalidMissingLevel = new PriorApprenticeshipQualification
            {
                StartDate = validStartDate,
                QualificationManualReasonCode = PriorApprenticeshipQualification.ManuallyEnteredCode,
                QualificationANZSCOCode = "anzsco"
            };
            validManuallyEntered = new PriorApprenticeshipQualification
            {
                StartDate = validStartDate,
                QualificationManualReasonCode = PriorApprenticeshipQualification.ManuallyEnteredCode,
                QualificationANZSCOCode = "anzsco",
                QualificationLevel = "level"
            };
            apprentice = new Profile {BirthDate = new DateTime(1980, 1, 1)};
            Container
                .GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorApprenticeshipQualificationsAsync(valid))
                .ReturnsAsync(new ValidationExceptionBuilder());
            Container
                .GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorApprenticeshipQualificationsAsync(invalidTooYoung))
                .ReturnsAsync(new ValidationExceptionBuilder());
            Container
                .GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorApprenticeshipQualificationsAsync(invalidFuture))
                .ReturnsAsync(new ValidationExceptionBuilder());
            Container
                .GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorApprenticeshipQualificationsAsync(invalidMissingAnzsco))
                .ReturnsAsync(new ValidationExceptionBuilder());
            Container
                .GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorApprenticeshipQualificationsAsync(invalidMissingLevel))
                .ReturnsAsync(new ValidationExceptionBuilder());
            Container
                .GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorApprenticeshipQualificationsAsync(validManuallyEntered))
                .ReturnsAsync(new ValidationExceptionBuilder());
            var builder = new ValidationExceptionBuilder();
            builder.AddException(ValidationExceptionType.InvalidPriorApprenticeshipAustralianStateCode);
            Container
                .GetMock<IReferenceDataValidator>()
                .Setup(r => r.ValidatePriorApprenticeshipQualificationsAsync(invalidRefCodes))
                .ReturnsAsync(builder);
        }

        [TestMethod]
        public async Task IsValidIfStartDateIsSensible()
        {
            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(valid, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
        }

        [TestMethod]
        public async Task IsValidIfManuallyEntered()
        {
            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(validManuallyEntered, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
        }

        [TestMethod]
        public async Task IsNotValidIfManualReasonCodeIsSomethingElse()
        {
            valid.QualificationManualReasonCode = "BLAH";
            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(valid, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidQualificationManualReasonCode);
        }

        [TestMethod]
        public async Task IsNotValidIfApprenticeWouldHaveBeenUnder12AtTheSpecifiedStartDate()
        {
            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(invalidTooYoung, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.DOBDateMismatch);
        }

        [TestMethod]
        public async Task IsNotValidIfStartDateIsInTheFuture()
        {
            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(invalidFuture, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidDate);
        }

        [TestMethod]
        public async Task IsNotValidIfReferenceCodesAreInvalid()
        {
            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(invalidRefCodes, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidPriorApprenticeshipAustralianStateCode);
        }

        [TestMethod]
        public async Task IsNotValidIfManualEntryAndAnzscoCodeMissing()
        {
            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(invalidMissingAnzsco, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidPriorApprenticeshipMissingAnzscoCode);
        }

        [TestMethod]
        public async Task IsNotValidIfManualEntryAndLevelCodeMissing()
        {
            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(invalidMissingLevel, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidPriorApprenticeshipMissingLevelCode);
        }
    }

    #endregion
}