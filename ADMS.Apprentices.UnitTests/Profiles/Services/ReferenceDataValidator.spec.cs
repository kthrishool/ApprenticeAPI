using System.Collections.Generic;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.HttpClients.ReferenceDataApi;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingReferenceData

    [TestClass]
    public class WhenValidatingReferenceDataValidator : GivenWhenThen<ReferenceDataValidator>
    {
        private Profile newProfile;
        private Qualification qualification;

        protected override void Given()
        {
            newProfile = new Profile();

            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});
        }

        private void ResetExceptionforExceptionValidation(ValidationExceptionType exception, Profile newProfile)
        {
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(this.newProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        private void MockReferenceData(string MethodName, IList<ListCodeResponseV1> returnvalue, ValidationExceptionType exception)
        {
            switch (MethodName)
            {
                case "GetListCodes":
                    Container
                        .GetMock<IReferenceDataClient>()
                        .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                        .ReturnsAsync(returnvalue);
                    break;
            }
        }

        [TestMethod]
        public void DoesNothingIfCountryofBirthIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidCountryCode);


            newProfile = new Profile();
            newProfile.CountryOfBirthCode = "1101";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }


        [TestMethod]
        public void ThrowsValidationExceptionIfCountryofBirthIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidCountryCode);


            newProfile = new Profile();
            newProfile.CountryOfBirthCode = "dasdas";


            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfLanguageisInValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidLanguageCode);


            newProfile = new Profile();
            newProfile.LanguageCode = "dasdas";


            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void DoNothingIfLanguageCodeIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1201", Description = "test",});


            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);

            newProfile = new Profile();
            newProfile.LanguageCode = "1201";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void DoesNothingIfIndegenousStatusIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidIndegenousStatusCode);

            newProfile = new Profile();
            newProfile.IndigenousStatusCode = "1101";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }


        [TestMethod]
        public void ThrowsValidationExceptionIfIndegenousStatusIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidIndegenousStatusCode);

            newProfile = new Profile();
            newProfile.IndigenousStatusCode = "dasdas";

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void DoesNothingIfCitizenshipCodeIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidCitizenshipCode);

            newProfile = new Profile();
            newProfile.CitizenshipCode = "1101";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }


        [TestMethod]
        public void ThrowsValidationExceptionIfCitizenshipCodeIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidCitizenshipCode);

            newProfile = new Profile();
            newProfile.CitizenshipCode = "dasdas";

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void DoesNothingIfLanguageIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1200", Description = "test",});

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidCountryCode);


            newProfile = new Profile();
            newProfile.CountryOfBirthCode = "1200";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void DoesNothingIfPreferredCodeTypeIsValid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = ProfileConstants.PreferredContactType.ToString();
            newProfile.Phones.Add(new Phone() {PhoneNumber = "0411111111"});
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenMobileContactTypeIsInvalid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = ProfileConstants.PreferredContactType.ToString();

            ResetExceptionforExceptionValidation(ValidationExceptionType.MobilePreferredContactIsInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenMobileContactTypeIsNotInCode()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = "test";

            ResetExceptionforExceptionValidation(ValidationExceptionType.InvalidPreferredContactCode, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenMobileContactTypeHasNoMobileNo()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = ProfileConstants.PreferredContactType.ToString();

            newProfile.Phones.Add(new Phone() {PhoneNumber = "0211111111"});

            ResetExceptionforExceptionValidation(ValidationExceptionType.MobilePreferredContactIsInvalid, newProfile);
        }

        [TestMethod]
        public void DoNothingWhenPhoneContactTypeIsValidAsync()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.PHONE.ToString();
            newProfile.Phones.Add(new Phone() {PhoneNumber = "0211111111"});
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void DoNothingWhenEmailContactTypeIsValid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.EMAIL.ToString();
            newProfile.EmailAddress = ProfileConstants.Emailaddress;
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void DoNothingWhenMailContactTypeIsValid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.MAIL.ToString();
            newProfile.Addresses.Add(new Address()
            {
                StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
                Postcode = ProfileConstants.ResidentialAddress.Postcode,
                Locality = ProfileConstants.ResidentialAddress.Locality,
                StateCode = ProfileConstants.ResidentialAddress.StateCode
            });
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).HasExceptions().Should().BeFalse());
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenPhoneContactTypeIsInvalid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.PHONE.ToString();

            ResetExceptionforExceptionValidation(ValidationExceptionType.PhonePreferredContactisInvalid, newProfile);
        }

        #region PreferredContactType

        [TestMethod]
        public void DOnothingIfPhoneIsPrefferedContactType()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.PHONE.ToString()};

            newProfile.Phones.Add(new Phone() {PhoneNumber = "0211111111"});

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(this.newProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void DOnothingIfEmailIsPrefferedContactType()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.EMAIL.ToString(), EmailAddress = ProfileConstants.Emailaddress};


            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(this.newProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void DOnothingIfMailIsPrefferedContactType()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.MAIL.ToString()};
            var localAddress = new Address()
            {
                StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
                StreetAddress2 = ProfileConstants.ResidentialAddress.StreetAddress2,
                StreetAddress3 = ProfileConstants.ResidentialAddress.StreetAddress3,
                Locality = ProfileConstants.ResidentialAddress.Locality,
                Postcode = ProfileConstants.ResidentialAddress.Postcode,
                StateCode = ProfileConstants.ResidentialAddress.StateCode,
                AddressTypeCode = AddressType.RESD.ToString()
            };
            newProfile.Addresses.Add(localAddress);

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(this.newProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        #endregion


        [TestMethod]
        public void ThrowValidationExceptionWhenMobileContactTypeAndNoPhoneDetails()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.MOBILE.ToString()};
            newProfile.Phones.Add(new Phone {PhoneNumber = null});
            ResetExceptionforExceptionValidation(ValidationExceptionType.MobilePreferredContactIsInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenSMSContactTypeIsInvalid()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.SMS.ToString()};
            newProfile.Phones.Add(new Phone() {PhoneNumber = "0211111111"});

            ResetExceptionforExceptionValidation(ValidationExceptionType.MobilePreferredContactIsInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenSMSContactTypeAndNoPhone()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.SMS.ToString()};
            newProfile.Phones.Clear();

            ResetExceptionforExceptionValidation(ValidationExceptionType.MobilePreferredContactIsInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenEmailContactTypeIsInvalid()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.EMAIL.ToString(), EmailAddress = null};

            ResetExceptionforExceptionValidation(ValidationExceptionType.EmailPreferredContactisInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenAddressContactTypeIsInvalid()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.MAIL.ToString()};
            newProfile.Addresses.Clear();

            ResetExceptionforExceptionValidation(ValidationExceptionType.MailPreferredContactisInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenAddressContactTypeAndNoAddress()
        {
            newProfile = new Profile {PreferredContactType = PreferredContactType.MAIL.ToString()};

            ResetExceptionforExceptionValidation(ValidationExceptionType.MailPreferredContactisInvalid, newProfile);
        }

        [TestMethod]
        public void DoesNothingIfSchoolLevelCodeIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "99", Description = "test",});

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidHighestSchoolLevelCode);

            newProfile = new Profile();
            newProfile.HighestSchoolLevelCode = "99";

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).ThrowAnyExceptions())
                .Should().NotThrow();
        }


        [TestMethod]
        public void ThrowsValidationExceptionIfSchoolLevelCodeIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidHighestSchoolLevelCode);

            newProfile = new Profile {HighestSchoolLevelCode = "invalidCode"};

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(newProfile)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }


        #region QualificationValidationUsingReferenceData

        [TestMethod]
        public void DoesNothingIfQualificationIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidQualificationLevel);

            qualification = ProfileConstants.Qualification;
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification)).ThrowAnyExceptions()).Should().NotThrow();
        }

        [TestMethod]
        public void ThrowsExceptionIfQualificationLevelIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();

            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);
            qualification = new Qualification();
            qualification.QualificationLevel = "Invalid";
            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfQualificationANZSCIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);
            qualification = new Qualification();
            qualification.QualificationANZSCOCode = "Invalid";

            ClassUnderTest.Invoking(async c => (await c.ValidateAsync(qualification)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        #endregion

        #region PriorApprenticeshipValidations

        [TestMethod]
        public void ThrowsExceptionIfCountryIsNull()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);
            var priorApprenticeship = ProfileConstants.PriorApprenticeship;


            ClassUnderTest.Invoking(async c => (await c.PriorApprenticeshipValidator(priorApprenticeship)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }


        [TestMethod]
        public void ThrowsExceptionIfCountryIsNotInReferenceData()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);
            var priorApprenticeship = ProfileConstants.PriorApprenticeship;
            priorApprenticeship.CountryCode = "2222";

            ClassUnderTest.Invoking(async c => (await c.PriorApprenticeshipValidator(priorApprenticeship)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfCountryIsAustraliaAndStateIsNull()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);
            var priorApprenticeship = ProfileConstants.PriorApprenticeship;
            priorApprenticeship.CountryCode = "1101";

            ClassUnderTest.Invoking(async c => (await c.PriorApprenticeshipValidator(priorApprenticeship)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void SHouldNotThrowExceptionWhenValidStateCode()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);
            var priorApprenticeship = ProfileConstants.PriorApprenticeship;
            priorApprenticeship.CountryCode = "1101";
            priorApprenticeship.StateCode = "ACT";
            ClassUnderTest.Invoking(async c => (await c.PriorApprenticeshipValidator(priorApprenticeship)).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }

        [TestMethod]
        public void throwExceptionWhenStateCodeIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);
            var priorApprenticeship = ProfileConstants.PriorApprenticeship;
            priorApprenticeship.CountryCode = "1101";
            priorApprenticeship.StateCode = "ACTa";
            ClassUnderTest.Invoking(async c => (await c.PriorApprenticeshipValidator(priorApprenticeship)).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        #endregion
    }

    #endregion
}