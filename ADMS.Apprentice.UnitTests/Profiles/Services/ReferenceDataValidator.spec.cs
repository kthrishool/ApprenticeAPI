using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.HttpClients.ReferenceDataApi;
using ADMS.Apprentice.Core.Services;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenValidatingAAddressValidator

    [TestClass]
    public class WhenValidatingReferenceDataValidator : GivenWhenThen<ReferenceDataValidator>
    {
        private Profile newProfile;
        private ValidationException validationException;
        private IReferenceDataClient testrefdata;

        protected override void Given()
        {
            newProfile = new Profile();

            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});

            //Container
            //    .GetMock<IReferenceDataClient>()
            //    .Setup(r => r.GetListCodes("22", "22", true, false, 0, "", true))
            //    .ReturnsAsync(list1);

            validationException = new ValidationException(null, (ValidationError) null);
            //Container
            //    .GetMock<IExceptionFactory>()
            //    .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidCountryCode))
            //    .Returns(validationException);
        }

        [TestMethod]
        public async Task DoesNothingIfCountryofBirthIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() {ShortDescription = "test", Code = "1101", Description = "test",});
            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);

            newProfile = new Profile();
            newProfile.CountryOfBirthCode = "1101";
            await ClassUnderTest.ValidateAsync(newProfile);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfCountryofBirthIsInValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();

            Container
                .GetMock<IReferenceDataClient>()
                .Setup(r => r.GetListCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync(list1);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidCountryCode))
                .Returns(validationException);

            newProfile = new Profile();
            newProfile.CountryOfBirthCode = "dasdas";
            //ClassUnderTest.ValidateAsync(newProfile)

            ClassUnderTest.Invoking(c => c.ValidateAsync(newProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }
    }

    #endregion
}