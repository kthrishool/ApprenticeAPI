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
    #region WhenValidatingReferenceData

    [TestClass]
    public class WhenValidatingReferenceDataValidator : GivenWhenThen<ReferenceDataValidator>
    {
        private Profile newProfile;
        private ValidationException validationException;

        protected override void Given()
        {
            newProfile = new Profile();
            validationException = new ValidationException(null, (ValidationError) null);

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
        public  void DoesNothingIfSchoolLevelCodeIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() { ShortDescription = "test", Code = "99", Description = "test", });

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

        [TestMethod]
        public void DoesNothingIfLeftSchoolMonthCodeIsValid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            list1.Add(new ListCodeResponseV1() { ShortDescription = "test", Code = "JAN", Description = "test", });

            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidMonthCode);

            newProfile = new Profile();
            newProfile.LeftSchoolMonthCode = "JAN";

            ClassUnderTest.Invoking(c => c.ValidateAsync(newProfile))
                .Should().NotThrow();
        }


        [TestMethod]
        public void ThrowsValidationExceptionIfLeftSchoolMonthCodeInvalid()
        {
            IList<ListCodeResponseV1> list1 = new List<ListCodeResponseV1>();
            MockReferenceData("GetListCodes", list1, ValidationExceptionType.InvalidMonthCode);

            newProfile = new Profile();
            newProfile.LeftSchoolMonthCode = "invalidCode";

            ClassUnderTest.Invoking(c => c.ValidateAsync(newProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }
    }

    #endregion
}