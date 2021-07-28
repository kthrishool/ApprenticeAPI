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
        private const string firstName = "Joe";
        private const string surname = "Bloggs";
        private ApprenticeIdentitySearchCriteriaMessage fullyPopulatedMessage;
        private readonly DateTime dob = new(1985, 12, 3);

        protected override void Given()
        {
            fullyPopulatedMessage = new ApprenticeIdentitySearchCriteriaMessage
            {
                PhoneNumber = phoneNumber,
                BirthDate = dob,
                EmailAddress = emailAddress,
                FirstName = firstName,
                Surname = surname,
                USI = usi
            };
        }

        [TestMethod]
        public void ThrowsAnExceptionIfNull()
        {
            ClassUnderTest.Invoking(c => c.Validate(null))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.InsufficientApprenticeIdentitySearchCriteria));
        }

        [TestMethod]
        public void ThrowsAnExceptionIfAllCriteriaAreNull()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage()))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.InsufficientApprenticeIdentitySearchCriteria));
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
        public void DoesNotAllowNamesOnly()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {Surname = surname, FirstName = firstName}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.FirstNameOrSurnameMustBeCombinedWithBirthDate));
        }

        [TestMethod]
        public void DoesNotAllowBirthDateOnly()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {BirthDate = dob}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.BirthDateMustBeCombinedWithFirstNameOrSurname));
        }

        [TestMethod]
        public void AllowsBirthDateAndFirstName()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {BirthDate = dob, FirstName = firstName});
        }

        [TestMethod]
        public void AllowsSurnameAndBirthDate()
        {
            ClassUnderTest.Validate(new ApprenticeIdentitySearchCriteriaMessage {Surname = surname, BirthDate = dob});
        }

        [TestMethod]
        public void DoesNotAllowBirthDateUnlessThereIsAFirstNameOrASurname()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {USI = usi, BirthDate = dob}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.BirthDateMustBeCombinedWithFirstNameOrSurname));
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {PhoneNumber = phoneNumber, BirthDate = dob}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.BirthDateMustBeCombinedWithFirstNameOrSurname));
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {EmailAddress = emailAddress, BirthDate = dob}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.BirthDateMustBeCombinedWithFirstNameOrSurname));
        }

        [TestMethod]
        public void DoesNotAllowFirstNameWithoutBirthDate()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {USI = usi, FirstName = firstName}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.FirstNameOrSurnameMustBeCombinedWithBirthDate));
        }

        [TestMethod]
        public void DoesNotAllowSurnameWithoutBirthDate()
        {
            ClassUnderTest.Invoking(c => c.Validate(new ApprenticeIdentitySearchCriteriaMessage {USI = usi, Surname = surname}))
                .Should().Throw<AdmsValidationException>()
                .Where(e => e.IsForValidationRule(ValidationExceptionType.FirstNameOrSurnameMustBeCombinedWithBirthDate));
        }
    }

    #endregion
}