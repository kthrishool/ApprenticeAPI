using System;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services.Validators;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingSearchCriteriaForAnApprenticeIdentityMatch

    [TestClass]
    public class WhenValidatingSearchCriteriaForAnApprenticeIdentityMatch : GivenWhenThen<SearchCriteriaValidator>
    {
        private const string usi = "123456789A";
        private const string phoneNumber = "123123123";
        private const string emailAddress = "joe@home.com";
        private ApprenticeIdentitySearchCriteriaMessage fullyPopulatedMessage;
        private readonly DateTime dob = new(1985, 12, 3);

        protected override void Given()
        {
            fullyPopulatedMessage = new ApprenticeIdentitySearchCriteriaMessage
            {
                PhoneNumber = phoneNumber,
                BirthDate = dob,
                EmailAddress = emailAddress,
                FirstName = "Joe",
                Surname = "Bloggs",
                USI = usi
            };
        }

        [TestMethod]
        public void ThrowsAnExceptionIfNull()
        {
            ClassUnderTest.Invoking(c => c.Validate(null))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.MissingApprenticeIdentitySearchCriteria));
        }

        [TestMethod]
        public void ThrowsAnExceptionIfAllCriteriaAreNull()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage()))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.MissingApprenticeIdentitySearchCriteria));
        }

        [TestMethod]
        public void DoesNothingIfAllCriteriaAreSupplied()
        {
            ClassUnderTest.Validate(fullyPopulatedMessage);
        }

        [TestMethod]
        public void AllowsUSIOnly()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {USI = usi});
        }

        [TestMethod]
        public void AllowsEmailAddressOnly()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {EmailAddress = emailAddress});
        }

        [TestMethod]
        public void AllowsPhoneNumberOnly()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {PhoneNumber = phoneNumber});
        }

        [TestMethod]
        public void DoesNotAllowNames()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {Surname = "Bloggs", FirstName = "Joe"}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.InsufficientApprenticeIdentitySearchCriteria));
        }

        [TestMethod]
        public void DoesNotAllowBirthDateOnly()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {BirthDate = dob}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.BirthDateMustBeCombinedWithOtherApprenticeIdentitySearchCriteria));
        }

        [TestMethod]
        public void DoesNotAllowBirthDateAndFirstNameOnly()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {BirthDate = dob, FirstName = "Joe"}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.BirthDateMustBeCombinedWithOtherApprenticeIdentitySearchCriteria));
        }

        [TestMethod]
        public void AllowsSurnameAndBirthDate()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {Surname = "Bloggs", BirthDate = dob});
        }

        [TestMethod]
        public void AllowsBirthDateAndUSI()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {USI = usi, BirthDate = dob});
        }

        [TestMethod]
        public void AllowsBirthDateAndPhoneNumber()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {PhoneNumber = phoneNumber, BirthDate = dob});
        }

        [TestMethod]
        public void AllowsBirthDateAndEmailAddress()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {EmailAddress = emailAddress, BirthDate = dob});
        }
    }

    #endregion
}