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
        private PriorApprenticeshipQualification invalidMissingState;
        private PriorApprenticeshipQualification validOverseas;

        protected override void Given()
        {
            valid = new PriorApprenticeshipQualification {StartDate = validStartDate, CountryCode = "1101", StateCode = "ACT"};
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
            invalidMissingState = new PriorApprenticeshipQualification {StartDate = validStartDate, CountryCode = "1101", StateCode = null};
            validOverseas = new PriorApprenticeshipQualification {StartDate = validStartDate, CountryCode = "999", StateCode = null};
            apprentice = new Profile {BirthDate = new DateTime(1980, 1, 1)};
            
            var builder = new ValidationExceptionBuilder();
            builder.AddException(ValidationExceptionType.InvalidPriorApprenticeshipAustralianStateCode);
        }

        [TestMethod]
        public void IsValidIfStartDateIsSensible()
        {
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(valid, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
        }

        [TestMethod]
        public void IsValidIfManuallyEntered()
        {
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(validManuallyEntered, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
        }

        [TestMethod]
        public void IsNotValidIfManualReasonCodeIsSomethingElse()
        {
            valid.QualificationManualReasonCode = "BLAH";
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(valid, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidQualificationManualReasonCode);
        }

        [TestMethod]
        public void IsNotValidIfApprenticeWouldHaveBeenUnder12AtTheSpecifiedStartDate()
        {
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(invalidTooYoung, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.DOBDateMismatch);
        }

        [TestMethod]
        public void IsNotValidIfStartDateIsInTheFuture()
        {
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(invalidFuture, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidDate);
        }

        [TestMethod]
        public void IsNotValidIfManualEntryAndAnzscoCodeMissing()
        {
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(invalidMissingAnzsco, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidPriorApprenticeshipMissingAnzscoCode);
        }

        [TestMethod]
        public void IsNotValidIfManualEntryAndLevelCodeMissing()
        {
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(invalidMissingLevel, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidPriorApprenticeshipMissingLevelCode);
        }

        [TestMethod]
        public void IsValidIfOverseasAndMissingState()
        {
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(validOverseas, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
        }

        [TestMethod]
        public void IsNotValidIfAustraliaAndMissingState()
        {
            ValidationExceptionBuilder exceptionBuilder = ClassUnderTest.Validate(invalidMissingState, apprentice);
            exceptionBuilder.GetValidationExceptions().Should().Contain(ValidationExceptionType.InvalidPriorQualificationMissingStateCode);
        }
    }

    #endregion
}