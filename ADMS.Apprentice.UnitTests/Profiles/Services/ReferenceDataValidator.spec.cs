﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ADMS.Apprentice.Core.Services.Validators;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenValidatingReferenceData

    [TestClass]
    public class WhenValidatingReferenceDataValidator : GivenWhenThen<ReferenceDataValidator>
    {
        private Profile newProfile;
        private ValidationException validationException;
        private Qualification qualification;

        protected override void Given()
        {
            newProfile = new Profile();

            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});
            validationException = new ValidationException(null, (ValidationError) null);
        }

        private void ResetExceptionforExceptionValidation(ValidationExceptionType exception, Profile newProfile)
        {
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(exception))
                .Returns(validationException);

            ClassUnderTest.Invoking(c => c.ValidateAsync(this.newProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
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

                    Container
                        .GetMock<IExceptionFactory>()
                        .Setup(r => r.CreateValidationException(exception))
                        .Returns(validationException);
                    break;
            }
        }

        [TestMethod]
        public async Task DoesNothingIfCountryofBirthIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidCountryCode);


            newProfile = new Profile();
            newProfile.CountryOfBirthCode = "1101";
            await ClassUnderTest.ValidateAsync(newProfile);
        }


        [TestMethod]
        public void ThrowsValidationExceptionIfCountryofBirthIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidCountryCode);


            newProfile = new Profile();
            newProfile.CountryOfBirthCode = "dasdas";


            ClassUnderTest.Invoking(c => c.ValidateAsync(newProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfLanguageisInValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidLanguageCode);


            newProfile = new Profile();
            newProfile.LanguageCode = "dasdas";


            ClassUnderTest.Invoking(c => c.ValidateAsync(newProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }


        [TestMethod]
        public async Task DoesNothingIfLanguageIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1200", Description = "test",});

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidCountryCode);


            newProfile = new Profile();
            newProfile.CountryOfBirthCode = "1200";
            await ClassUnderTest.ValidateAsync(newProfile);
        }

        [TestMethod]
        public async Task DoesNothingIfPreferredCodeTypeIsValid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = ProfileConstants.PreferredContactType.ToString();
            newProfile.Phones.Add(new Phone() {PhoneNumber = "0411111111"});
            await ClassUnderTest.ValidateAsync(newProfile);
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
        public async Task DoNothingWhenPhoneContactTypeIsValidAsync()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.PHONE.ToString();
            newProfile.Phones.Add(new Phone() { PhoneNumber = "0211111111" });
            await ClassUnderTest.ValidateAsync(newProfile); 
        }
        [TestMethod]
        public async Task DoNothingWhenEmailContactTypeIsValid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.EMAIL.ToString();
            newProfile.EmailAddress = ProfileConstants.Emailaddress;
            await ClassUnderTest.ValidateAsync(newProfile);
        }
        [TestMethod]
        public async Task DoNothingWhenMailContactTypeIsValid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.MAIL.ToString();
            newProfile.Addresses.Add(new Address() { 
            StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
            Postcode  = ProfileConstants.ResidentialAddress.Postcode,
            Locality = ProfileConstants.ResidentialAddress.Locality,
            StateCode = ProfileConstants.ResidentialAddress.StateCode
            } );
            await ClassUnderTest.ValidateAsync(newProfile);
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
        


            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.PHONE.ToString();

            newProfile.Phones.Add(new Phone() { PhoneNumber = "0211111111" });


            ClassUnderTest.Invoking(c => c.ValidateAsync(this.newProfile))
                .Should().NotThrow();
        }

        [TestMethod]
        public void DOnothingIfEmailIsPrefferedContactType()
        {
    


            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.EMAIL.ToString();

            newProfile.EmailAddress = ProfileConstants.Emailaddress;


            ClassUnderTest.Invoking(c => c.ValidateAsync(this.newProfile))
                .Should().NotThrow();
        }

        [TestMethod]
        public void DOnothingIfMailIsPrefferedContactType()
        {
            


            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.MAIL.ToString();
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


            ClassUnderTest.Invoking(c => c.ValidateAsync(this.newProfile))
                .Should().NotThrow();
        }

        #endregion



        [TestMethod]
        public void ThrowValidationExceptionWhenPhoneContactTypeAndNoPhone()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.PHONE.ToString();
            newProfile.Phones = null;
            ResetExceptionforExceptionValidation(ValidationExceptionType.PhonePreferredContactisInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenSMSContactTypeIsInvalid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.SMS.ToString();

            newProfile.Phones.Add(new Phone() {PhoneNumber = "0211111111"});

            ResetExceptionforExceptionValidation(ValidationExceptionType.MobilePreferredContactIsInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenSMSContactTypeAndNoPhone()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.SMS.ToString();

            newProfile.Phones = null;

            ResetExceptionforExceptionValidation(ValidationExceptionType.MobilePreferredContactIsInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenEmailContactTypeIsInvalid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.EMAIL.ToString();

            newProfile.EmailAddress = null;


            ResetExceptionforExceptionValidation(ValidationExceptionType.EmailPreferredContactisInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenAddressContactTypeIsInvalid()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.MAIL.ToString();

            newProfile.Addresses = null;

            ResetExceptionforExceptionValidation(ValidationExceptionType.MailPreferredContactisInvalid, newProfile);
        }

        [TestMethod]
        public void ThrowValidationExceptionWhenAddressContactTypeAndNoAddress()
        {
            newProfile = new Profile();
            newProfile.PreferredContactType = PreferredContactType.MAIL.ToString();

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

            ClassUnderTest.Invoking(c => c.ValidateAsync(newProfile))
                .Should().NotThrow();
        }


        [TestMethod]
        public void ThrowsValidationExceptionIfSchoolLevelCodeIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidHighestSchoolLevelCode);

            newProfile = new Profile();
            newProfile.HighestSchoolLevelCode = "invalidCode";

            ClassUnderTest.Invoking(c => c.ValidateAsync(newProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }



        #region QualificationValidationUsingReferenceData
        [TestMethod]
        public void DoesNothingIfQualificationIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() { ShortDescription = "test", Code = "1101", Description = "test", });

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidQualification);

            qualification = ProfileConstants.Qualification;            
            ClassUnderTest.Invoking(c => c.ValidateAsync(qualification)).Should().NotThrow();
        }

        [TestMethod]
        public void ThrowsExceptionIfQualificationLevelIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();

            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidQualificationLevel))
                .Returns(validationException);
            qualification = new Qualification();
            qualification.QualificationLevel = "Invalid";
            ClassUnderTest.Invoking(c => c.ValidateAsync(qualification))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfQualificationANZSCIsInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>(); 
            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidQualificationANZSCO))
                .Returns(validationException);

            qualification = new Qualification();
            qualification.QualificationANZSCOCode = "Invalid";
            
            ClassUnderTest.Invoking(c => c.ValidateAsync(qualification))
                .Should().Throw<ValidationException>();
        }
        #endregion
    }

    #endregion    
}