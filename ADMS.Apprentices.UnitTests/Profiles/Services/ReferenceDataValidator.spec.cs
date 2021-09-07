//using System.Collections.Generic;
//using System.Threading.Tasks;
//using ADMS.Apprentices.Core.Entities;
//using ADMS.Apprentices.Core.HttpClients.ReferenceDataApi;
//using ADMS.Apprentices.Core.Services.Validators;
//using Adms.Shared.Testing;
//using FluentAssertions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace ADMS.Apprentices.UnitTests.Profiles.Services
//{
//    #region WhenValidatingAnApprenticeProfileWithTheReferenceDataValidator

//    [TestClass]
//    public class WhenValidatingAnApprenticeProfileWithTheReferenceDataValidator : GivenWhenThen<ReferenceDataValidator>
//    {
//        private const string validCode = "valid-code";
//        private const string invalidCode = "invalid-code";

//        protected override void Given()
//        {
//            var validCodes = new List<ListCodeResponseV1> {new() {Code = validCode}};
//            Container
//                .GetMock<IReferenceDataClient>()
//                .Setup(r => r.GetListCodes(It.IsAny<string>(), validCode, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
//                .ReturnsAsync(validCodes);
//            Container
//                .GetMock<IReferenceDataClient>()
//                .Setup(r => r.GetListCodes(It.IsAny<string>(), invalidCode, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
//                .ReturnsAsync(new List<ListCodeResponseV1>());
//        }

//        [TestMethod]
//        public async Task DoesNothingIfCountryofBirthIsValid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {CountryOfBirthCode = validCode});
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfCountryofBirthIsInvalid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {CountryOfBirthCode = invalidCode});
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfLanguageCodeIsValid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {LanguageCode = validCode});
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfLanguageCodeIsInvalid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {LanguageCode = invalidCode});
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfIndigenousStatusCodeIsValid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {IndigenousStatusCode = validCode});
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfIndigenousStatusCodeIsInvalid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {IndigenousStatusCode = invalidCode});
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfNotProvidingUSIReasonCodeIsValid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {NotProvidingUSIReasonCode = validCode});
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfNotProvidingUSIReasonCodeIsInvalid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {NotProvidingUSIReasonCode = invalidCode});
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfCitizenshipCodeIsValid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {CitizenshipCode = validCode});
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfCitizenshipCodeIsInvalid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {CitizenshipCode = invalidCode});
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfCountryOfBirthCodeIsValid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {CountryOfBirthCode = validCode});
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfCountryOfBirthCodeIsInvalid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {CountryOfBirthCode = invalidCode});
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfHighestSchoolLevelCodeIsValid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {HighestSchoolLevelCode = validCode});
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfHighestSchoolLevelCodeIsInvalid()
//        {
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidateAsync(new Profile {HighestSchoolLevelCode = invalidCode});
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }
//    }

//    #endregion

//    #region WhenValidatingPriorQualificationsWithTheReferenceDataValidator

//    [TestClass]
//    public class WhenValidatingPriorQualificationsWithTheReferenceDataValidator : GivenWhenThen<ReferenceDataValidator>
//    {
//        private const string validCode = "valid-code";
//        private const string invalidCode = "invalid-code";
//        private PriorQualification qualification;

//        protected override void Given()
//        {
//            var validCodes = new List<ListCodeResponseV1> {new() {Code = validCode}};
//            Container
//                .GetMock<IReferenceDataClient>()
//                .Setup(r => r.GetListCodes(It.IsAny<string>(), validCode, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
//                .ReturnsAsync(validCodes);
//            Container
//                .GetMock<IReferenceDataClient>()
//                .Setup(r => r.GetListCodes(It.IsAny<string>(), invalidCode, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
//                .ReturnsAsync(new List<ListCodeResponseV1>());
//            qualification = new PriorQualification();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfQualificationLevelIsValid()
//        {
//            qualification.QualificationLevel = validCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfQualificationLevelIsInvalid()
//        {
//            qualification.QualificationLevel = invalidCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfQualificationANZSCOCodeIsValid()
//        {
//            qualification.QualificationANZSCOCode = validCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfQualificationANZSCOCodeIsInvalid()
//        {
//            qualification.QualificationANZSCOCode = invalidCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }
//    }

//    #endregion

//    #region WhenValidatingPriorApprenticeshipQualificationsWithTheReferenceDataValidator

//    [TestClass]
//    public class WhenValidatingPriorApprenticeshipQualificationsWithTheReferenceDataValidator : GivenWhenThen<ReferenceDataValidator>
//    {
//        private const string validCode = "valid-code";
//        private const string invalidCode = "invalid-code";
//        private PriorApprenticeshipQualification qualification;

//        protected override void Given()
//        {
//            var validCodes = new List<ListCodeResponseV1> {new() {Code = validCode}};
//            Container
//                .GetMock<IReferenceDataClient>()
//                .Setup(r => r.GetListCodes(It.IsAny<string>(), validCode, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
//                .ReturnsAsync(validCodes);
//            Container
//                .GetMock<IReferenceDataClient>()
//                .Setup(r => r.GetListCodes(It.IsAny<string>(), invalidCode, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
//                .ReturnsAsync(new List<ListCodeResponseV1>());
//            qualification = new PriorApprenticeshipQualification();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfCountryCodeIsValid()
//        {
//            qualification.CountryCode = validCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfCountryCodeIsEmpty()
//        {
//            qualification.CountryCode = null;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfCountryCodeIsInvalid()
//        {
//            qualification.CountryCode = invalidCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfStateCodeIsValid()
//        {
//            qualification.StateCode = "ACT";
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfStateCodeIsEmpty()
//        {
//            qualification.StateCode = null;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfStateCodeIsInvalid()
//        {
//            qualification.StateCode = invalidCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfQualificationLevelIsValid()
//        {
//            qualification.QualificationLevel = validCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfQualificationLevelIsEmpty()
//        {
//            qualification.QualificationLevel = null;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfQualificationLevelIsInvalid()
//        {
//            qualification.QualificationLevel = invalidCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfQualificationANZSCOCodeIsValid()
//        {
//            qualification.QualificationANZSCOCode = validCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task DoesNothingIfQualificationANZSCOCodeIsEmpty()
//        {
//            qualification.QualificationANZSCOCode = null;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().BeEmpty();
//        }

//        [TestMethod]
//        public async Task ReturnsExceptionIfQualificationANZSCOCodeIsInvalid()
//        {
//            qualification.QualificationANZSCOCode = invalidCode;
//            ValidationExceptionBuilder exceptionBuilder = await ClassUnderTest.ValidatePriorApprenticeshipQualificationsAsync(qualification);
//            exceptionBuilder.GetValidationExceptions().Should().NotBeEmpty();
//        }
//    }

//    #endregion
//}