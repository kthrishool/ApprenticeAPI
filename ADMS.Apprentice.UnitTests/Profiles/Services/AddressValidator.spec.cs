using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.UnitTests.Constants;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenValidatingAAddressValidator

    [TestClass]
    public class WhenValidatingAAddressValidator : GivenWhenThen<AddressValidator>
    {
        private Address validAddress;
        private Address invalidAddress;
        private ValidationException validationException;

        protected override void Given()
        {
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
                SingleLineAddress = ProfileConstants.ResidentialAddress.SingleLineAddress,
                AddressTypeCode = AddressType.RESD.ToString()
            };

            validationException = new ValidationException(null, (ValidationError) null);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.AddressRecordNotFound))
                .Returns(validationException);
        }

        protected override async void When()
        {
            await ClassUnderTest.ManualAddressValidator(validAddress);
        }

        //[TestMethod]
        //public async Task DoesNothingIfTheAddressIsValid()
        //{
        //    await ClassUnderTest.ManualAddressValidator(validAddress);
        //}

        //[TestMethod]
        //public void ThrowsValidationExceptionIfAddressIsInValid()
        //{
        //    ClassUnderTest
        //        .Invoking(async c => await c.ManualAddressValidator(invalidAddress))
        //        .Should().Throw<ValidationException>();
        //}

        //[TestMethod]
        //public void ThrowsValidationExceptionIfPostCodeLocalityMissMatch()
        //{
        //    invalidAddress.Postcode = "2601";
        //    invalidAddress.SingleLineAddress = "";
        //    ClassUnderTest
        //        .Invoking(async c => await c.ManualAddressValidator(invalidAddress))
        //        .Should().Throw<ValidationException>();
        //}
    }

    #endregion
}