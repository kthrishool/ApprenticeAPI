using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenValidatingAAddressValidator

    [TestClass]
    public class WhenValidatingAAddressValidator : GivenWhenThen<AddressValidator>
    {
        private Address validAddress;
        private Address invalidAddress;
        private Profile newProfile;
        private ValidationException validationException;

        protected override void Given()
        {
            newProfile = new Profile();
            validAddress = new Address()
            {
                StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
                StreetAddress2 = ProfileConstants.ResidentialAddress.StreetAddress2,
                StreetAddress3 = ProfileConstants.ResidentialAddress.StreetAddress3,
                Locality = ProfileConstants.ResidentialAddress.Locality,
                Postcode = ProfileConstants.ResidentialAddress.Postcode,
                StateCode = ProfileConstants.ResidentialAddress.StateCode,
                // SingleLineAddress = ProfileConstants.ResidentialAddress.SingleLineAddress,
                AddressTypeCode = AddressType.RESD.ToString()
            };
            invalidAddress = new Address()
            {
                StreetAddress1 = ProfileConstants.ResidentialAddress.StreetAddress1,
                StreetAddress2 = ProfileConstants.ResidentialAddress.StreetAddress2,
                StreetAddress3 = ProfileConstants.ResidentialAddress.StreetAddress3,
                Locality = ProfileConstants.ResidentialAddress.Locality,
                Postcode = "",
                StateCode = "DONT KNOW",
                // SingleLineAddress = ProfileConstants.ResidentialAddress.SingleLineAddress,
                AddressTypeCode = AddressType.RESD.ToString()
            };
            newProfile.Addresses.Add(validAddress);
            validationException = new ValidationException(null, (ValidationError) null);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.AddressRecordNotFound))
                .Returns(validationException);
        }

        private Address UpdateAddressColumn(string columnName, string value, ValidationExceptionType exception, bool RaiseException)
        {
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
            switch (columnName)
            {
                case "StreetAddress1":
                    localAddress.StreetAddress1 = value;
                    break;
                case "StreetAddress2":
                    localAddress.StreetAddress2 = value;
                    break;
                case "StreetAddress3":
                    localAddress.StreetAddress3 = value;
                    break;
                case "Locality":
                    localAddress.Locality = value;
                    break;
                case "StateCode":
                    localAddress.StateCode = value;
                    break;
                case "AddressTypeCode":
                    localAddress.AddressTypeCode = value;
                    break;
                case "Postcode":
                    localAddress.Postcode = value;
                    break;
            }

            newProfile.Addresses = new List<Address>();

            newProfile.Addresses.Add(localAddress);

            if (RaiseException)
            {
                Container
                    .GetMock<IExceptionFactory>()
                    .Setup(r => r.CreateValidationException(exception))
                    .Returns(validationException);


                ClassUnderTest
                    .Invoking(c => c.Validate(newProfile))
                    .Should().Throw<ValidationException>();
            }
            else
            {
                ClassUnderTest.Validate(newProfile);
            }

            return localAddress;
        }

        protected override async void When()
        {
            ClassUnderTest.Validate(newProfile);
        }

        [TestMethod]
        public void DoesNothingIfTheAddressIsValid()
        {
            ClassUnderTest.Validate(newProfile);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfAddressIsInValid()
        {
            newProfile.Addresses = new List<Address>();
            newProfile.Addresses.Add(invalidAddress);
            ClassUnderTest.Invoking(c => c.Validate(newProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfPostCodeLocalityMissMatch()
        {
            UpdateAddressColumn("Postcode", "260", ValidationExceptionType.InvalidPostcode, true);
            UpdateAddressColumn("Postcode", "26022", ValidationExceptionType.InvalidPostcode, true);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfStateCodeExceedMaxlength()
        {
            UpdateAddressColumn("StateCode", ProfileConstants.RandomString(23), ValidationExceptionType.InvalidStateCode, true);

            UpdateAddressColumn("StateCode", "", ValidationExceptionType.AddressRecordNotFound, true);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfStreetLine1IsNull()
        {
            UpdateAddressColumn("StreetAddress1", null, ValidationExceptionType.StreetAddressLine1CannotBeNull, true);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfStreetLineExceedsLength()
        {
            UpdateAddressColumn("StreetAddress1", ProfileConstants.RandomString(82), ValidationExceptionType.StreetAddressExceedsMaxLength, true);
            UpdateAddressColumn("StreetAddress2", ProfileConstants.RandomString(82), ValidationExceptionType.StreetAddressExceedsMaxLength, true);
            UpdateAddressColumn("StreetAddress3", ProfileConstants.RandomString(82), ValidationExceptionType.StreetAddressExceedsMaxLength, true);
        }


        [TestMethod]
        public void DoNothingIfStreetLine23IsNull()
        {
            UpdateAddressColumn("StreetAddress2", null, ValidationExceptionType.StreetAddressLine1CannotBeNull, false);
            UpdateAddressColumn("StreetAddress3", null, ValidationExceptionType.StreetAddressLine1CannotBeNull, false);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfLocalityExceedsLenght()
        {
            UpdateAddressColumn("Locality", ProfileConstants.RandomString(42), ValidationExceptionType.SuburbExceedsMaxLength, true);
        }
    }

    #endregion
}