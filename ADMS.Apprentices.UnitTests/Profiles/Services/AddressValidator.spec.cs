using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.HttpClients.ReferenceDataApi;
using ADMS.Apprentices.Core.Services.Validators;
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
    #region WhenValidatingAAddressValidator

    [TestClass]
    public class WhenValidatingAAddressValidator : GivenWhenThen<AddressValidator>
    {
        private Address validAddress;
        private Address invalidAddress;
        private Address validSingleLineAddress;
        private Address invalidSingleLineAddress;
        private Profile newProfile;
        private DetailAddressModel detailsAddress;
        private PartialAddressModel partialAddress;
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
                SingleLineAddress = ProfileConstants.ResidentialAddress.SingleLineAddress,
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
                AddressTypeCode = AddressType.RESD.ToString()
            };
            validSingleLineAddress = new Address() {SingleLineAddress = ProfileConstants.ResidentialSingleLineAddress.SingleLineAddress, AddressTypeCode = AddressType.RESD.ToString()};
            invalidSingleLineAddress = new Address() {SingleLineAddress = "invalidAddress", AddressTypeCode = AddressType.RESD.ToString()};

            detailsAddress = new DetailAddressModel(
                1, "Bname", "", ProfileConstants.ResidentialAddress.StreetAddress1,
                ProfileConstants.ResidentialAddress.StreetAddress1,
                ProfileConstants.ResidentialAddress.StreetAddress2,
                ProfileConstants.ResidentialAddress.StreetAddress3,
                ProfileConstants.ResidentialAddress.Locality,
                ProfileConstants.ResidentialAddress.StateCode,
                ProfileConstants.ResidentialAddress.Postcode,
                "GeoCode", (decimal) 32.56, (decimal) 45.4, "AddressSource", "SOurceId",
                2, 1, null, 1);

            partialAddress = new PartialAddressModel(
                ProfileConstants.ResidentialAddress.Locality,
                ProfileConstants.ResidentialAddress.StateCode,
                ProfileConstants.ResidentialAddress.Postcode,
                (decimal) 32.56, (decimal) 45.4, null);


            validationException = new ValidationException(null, (ValidationError) null);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.AddressRecordNotFound))
                .Returns(validationException);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(It.IsAny<ValidationExceptionType>()))
                .Returns(validationException);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(It.IsAny<ValidationExceptionType[]>()))
                .Returns(validationException);

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetDetailAddressByFormattedAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(detailsAddress);

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetAddressByFormattedLocality(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(partialAddress);
        }

        private async Task UpdateAddressColumnAsync(string columnName, string value, ValidationExceptionType exception, bool RaiseException)
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

            newProfile.Addresses.Clear();
            newProfile.Addresses.Add(localAddress);

            if (RaiseException)
            {
                Container
                    .GetMock<IExceptionFactory>()
                    .Setup(r => r.CreateValidationException(exception))
                    .Returns(validationException);

                ClassUnderTest
                    .Invoking(async c =>
                    {
                        Address address = newProfile.Addresses.SingleOrDefault();
                        (await c.ValidateAsync(address)).ThrowAnyExceptions();
                    })
                    .Should().Throw<ValidationException>();
            }
            else
            {
                (await ClassUnderTest.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions();
            }
        }

        protected override async void When()
        {
            foreach (Address newProfileAddress in newProfile.Addresses)
            {
                (await ClassUnderTest.ValidateAsync(newProfileAddress)).ThrowAnyExceptions();
            }
        }

        [TestMethod]
        public void DoesNothingIfTheAddressIsValid()
        {
            newProfile.Addresses.Add(validAddress);
            ClassUnderTest.Invoking(async c =>
                {
                    Address address = newProfile.Addresses.SingleOrDefault();
                    (await c.ValidateAsync(address)).ThrowAnyExceptions();
                })
                .Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfAddressIsInValid()
        {
            newProfile.Addresses.Clear();
            newProfile.Addresses.Add(invalidAddress);
            ClassUnderTest.Invoking(async c =>
                {
                    Address address = newProfile.Addresses.SingleOrDefault();
                    (await c.ValidateAsync(address)).ThrowAnyExceptions();
                })
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public async Task ThrowsValidationExceptionIfPostCodeLocalityMissMatchAsync()
        {
            await UpdateAddressColumnAsync("Postcode", "260", ValidationExceptionType.InvalidPostcode, true);
            await UpdateAddressColumnAsync("Postcode", "26022", ValidationExceptionType.InvalidPostcode, true);
        }

        [TestMethod]
        public async Task ThrowsValidationExceptionIfStateCodeExceedMaxlength()
        {
            await UpdateAddressColumnAsync("StateCode", ProfileConstants.RandomString(23), ValidationExceptionType.InvalidStateCode, true);

            await UpdateAddressColumnAsync("StateCode", "", ValidationExceptionType.AddressRecordNotFound, true);
        }

        [TestMethod]
        public async Task ThrowsValidationExceptionIfStreetLine1IsNull()
        {
            await UpdateAddressColumnAsync("StreetAddress1", null, ValidationExceptionType.StreetAddressLine1CannotBeNull, true);
            await UpdateAddressColumnAsync("StreetAddress1", "", ValidationExceptionType.StreetAddressLine1CannotBeNull, true);
            await UpdateAddressColumnAsync("StreetAddress1", " ", ValidationExceptionType.StreetAddressLine1CannotBeNull, true);
        }

        [TestMethod]
        public async Task ThrowsValidationExceptionIfStreetLineExceedsLength()
        {
            await UpdateAddressColumnAsync("StreetAddress1", ProfileConstants.RandomString(82), ValidationExceptionType.StreetAddressExceedsMaxLength, true);
            await UpdateAddressColumnAsync("StreetAddress2", ProfileConstants.RandomString(82), ValidationExceptionType.StreetAddressExceedsMaxLength, true);
            await UpdateAddressColumnAsync("StreetAddress3", ProfileConstants.RandomString(82), ValidationExceptionType.StreetAddressExceedsMaxLength, true);
        }


        [TestMethod]
        public async Task DoNothingIfStreetLine23IsNull()
        {
            await UpdateAddressColumnAsync("StreetAddress2", null, ValidationExceptionType.StreetAddressLine1CannotBeNull, false);
            await UpdateAddressColumnAsync("StreetAddress3", null, ValidationExceptionType.StreetAddressLine1CannotBeNull, false);
        }

        [TestMethod]
        public async Task ThrowsValidationExceptionIfLocalityExceedsLenght()
        {
            await UpdateAddressColumnAsync("Locality", ProfileConstants.RandomString(42), ValidationExceptionType.SuburbExceedsMaxLength, true);
        }

        #region iGasValidation

        [TestMethod]
        public async Task PopulateGeoLocationIfManualAddressIsValid()
        {
            newProfile = new Profile();
            newProfile.Addresses.Add(validAddress);


            await ClassUnderTest.ValidateAsync(newProfile.Addresses.SingleOrDefault());

            newProfile.Addresses.FirstOrDefault().Latitude.Should().NotBe(null);
            newProfile.Addresses.FirstOrDefault().Longitude.Should().NotBe(null);
            newProfile.Addresses.FirstOrDefault().Locality.Should().NotBe(null);
            newProfile.Addresses.FirstOrDefault().StateCode.Should().NotBe(null);
            newProfile.Addresses.FirstOrDefault().Postcode.Should().NotBe(null);
        }

        [TestMethod]
        public void ThrowsExceptionIfAddressIsNull()
        {
            newProfile = new Profile();
            invalidAddress = null;
            newProfile.Addresses.Add(invalidAddress);

            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfManualAddressIsInvalid()
        {
            newProfile = new Profile();
            invalidAddress = new Address()
            {
                StreetAddress1 = "invalid",
                StreetAddress2 = null,
                StreetAddress3 = null,
                Locality = "Locality",
                Postcode = "2020",
                StateCode = "DONT KNOW",
                AddressTypeCode = AddressType.RESD.ToString()
            };
            newProfile.Addresses.Add(invalidAddress);
            partialAddress = null;

            ThrowExceptionForAddressValidation();
        }

        private void ThrowExceptionForAddressValidation()
        {
            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetAddressByFormattedLocality(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(partialAddress);

            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfManualAddressIsInvalid1()
        {
            newProfile = new Profile();
            invalidAddress = new Address()
            {
                StreetAddress1 = "invalid",
                StreetAddress2 = null,
                StreetAddress3 = null,
                Locality = "Locality",
                Postcode = "2020",
                StateCode = "DONT KNOW",
                AddressTypeCode = AddressType.RESD.ToString()
            };
            newProfile.Addresses.Add(invalidAddress);

            partialAddress = new PartialAddressModel(
                null,
                ProfileConstants.ResidentialAddress.StateCode,
                ProfileConstants.ResidentialAddress.Postcode,
                (decimal) 0.0, (decimal) 0.0, null);

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetAddressByFormattedLocality(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(partialAddress);

            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }


        [TestMethod]
        public void ThrowsLocalityMismatchExceptionIfManualAddressIsInvalid1()
        {
            newProfile = new Profile();
            invalidAddress = new Address()
            {
                StreetAddress1 = "14 Mort street",
                StreetAddress2 = null,
                StreetAddress3 = null,
                Locality = "Locality",
                Postcode = "2601",
                StateCode = "ACT",
                AddressTypeCode = AddressType.RESD.ToString()
            };
            newProfile.Addresses.Add(invalidAddress);

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetAddressByFormattedLocality(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(partialAddress);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.PostCodeLocalityMismatch))
                .Returns(validationException);

            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsStateMismatchExceptionIfManualAddressStateIsInvalid1()
        {
            newProfile = new Profile();
            invalidAddress = new Address()
            {
                StreetAddress1 = "14 Mort street",
                StreetAddress2 = null,
                StreetAddress3 = null,
                Locality = "Braddon",
                Postcode = "2601",
                StateCode = "StateCode",
                AddressTypeCode = AddressType.RESD.ToString()
            };
            newProfile.Addresses.Add(invalidAddress);

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetAddressByFormattedLocality(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(partialAddress);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.PostCodeStateCodeMismatch))
                .Returns(validationException);

            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsPostCodeMismatchExceptionIfManualAddressPostCodeIsInvalid1()
        {
            newProfile = new Profile();
            invalidAddress = new Address()
            {
                StreetAddress1 = "14 Mort street",
                StreetAddress2 = null,
                StreetAddress3 = null,
                Locality = "Braddon",
                Postcode = "0000",
                StateCode = "ACT",
                AddressTypeCode = AddressType.RESD.ToString()
            };
            newProfile.Addresses.Add(invalidAddress);

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetAddressByFormattedLocality(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(partialAddress);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.PostCodeMismatch))
                .Returns(validationException);

            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public async Task PopulateGeoLocationIfSingleLineAddressIsValid()
        {
            newProfile = new Profile();
            newProfile.Addresses.Add(validSingleLineAddress);

            (await ClassUnderTest.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions();

            newProfile.Addresses.FirstOrDefault().Latitude.Should().NotBe(null);
            newProfile.Addresses.FirstOrDefault().Longitude.Should().NotBe(null);
            newProfile.Addresses.FirstOrDefault().Locality.Should().NotBe(null);
            newProfile.Addresses.FirstOrDefault().StateCode.Should().NotBe(null);
            newProfile.Addresses.FirstOrDefault().Postcode.Should().NotBe(null);
        }


        [TestMethod]
        public void ShouldNotThrowErrorIfSingleLineAddressIsValid()
        {
            newProfile = new Profile();
            newProfile.Addresses.Add(validSingleLineAddress);
            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().NotThrow();
        }

        [TestMethod]
        public void ThrowsExceptionIfSingleLineAddressIsInvalid()
        {
            newProfile = new Profile();
            newProfile.Addresses.Add(invalidSingleLineAddress);
            detailsAddress = null;

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetDetailAddressByFormattedAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(detailsAddress);

            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void ThrowsExceptionIfSingleLineAddressIsInvalid1()
        {
            newProfile = new Profile();
            newProfile.Addresses.Add(invalidSingleLineAddress);
            detailsAddress = new DetailAddressModel(
                1, "Bname", "", ProfileConstants.ResidentialAddress.StreetAddress1,
                ProfileConstants.ResidentialAddress.StreetAddress1,
                ProfileConstants.ResidentialAddress.StreetAddress2,
                ProfileConstants.ResidentialAddress.StreetAddress3,
                null, null, null,
                "GeoCode", (decimal) 32.56, (decimal) 45.4, "AddressSource", "SOurceId",
                2, 1, null, 1);

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetDetailAddressByFormattedAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(detailsAddress);

            ClassUnderTest
                .Invoking(async c => (await c.ValidateAsync(newProfile.Addresses.SingleOrDefault())).ThrowAnyExceptions())
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public async Task ThrowsReturnMultipleErrorsIfMismatchExceptionIfManualAddressPostCodeIsInvalid1()
        {
            newProfile = new Profile();
            invalidAddress = new Address()
            {
                StreetAddress1 = "14 Mort street",
                StreetAddress2 = null,
                StreetAddress3 = null,
                Locality = "Braddon",
                Postcode = "0000",
                StateCode = "Invalid",
                AddressTypeCode = AddressType.RESD.ToString()
            };
            newProfile.Addresses.Add(invalidAddress);

            Container.GetMock<IReferenceDataClient>()
                .Setup(x => x.GetAddressByFormattedLocality(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(partialAddress);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.PostCodeMismatch))
                .Returns(validationException);

            /* Slightly more complex code test for code coverage */
            var validationExceptionBuilder = new ValidationExceptionBuilder(Container.GetMock<IExceptionFactory>().Object);
            validationExceptionBuilder.AddExceptions((await ClassUnderTest.ValidateAsync(newProfile.Addresses.SingleOrDefault())).GetValidationExceptions());
            validationExceptionBuilder.GetValidationExceptions().Should().HaveCountGreaterThan(1);
        }
        #endregion

        #endregion
    }
}