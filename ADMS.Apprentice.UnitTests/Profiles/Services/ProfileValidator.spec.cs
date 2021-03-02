using System.Threading.Tasks;
using ADMS.Apprentice.Core;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenValidatingAClaimSubmission

    [TestClass]
    public class WhenValidatingAProfile : GivenWhenThen<ProfileValidator>
    {
        private Profile validProfile;
        private Profile invalidProfile;
        private ValidationException validationException;

        protected override void Given()
        {
            validProfile = new Profile
            {
                Surname = "Bob",
                FirstName = "Alex",
                BirthDate = DateTime.Now.AddYears(-25)
            };
            invalidProfile = new Profile
            {
                Surname = "Bob",
                FirstName = "Alex",
                BirthDate = DateTime.Now.AddYears(-2)
            };
            validationException = new ValidationException(null, (ValidationError)null);
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidApprenticeAge))
                .Returns(validationException);
        }

        [TestMethod]
        public async Task DoesNothingIfTheProfileIsValid()
        {
            await ClassUnderTest.ValidateAsync(validProfile);
        }

        [TestMethod]
        public void ThrowsValidationExceptionIfAgeIsLessThan12()
        {
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(invalidProfile))
                .Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void GetsTheValidationExceptionFromTheExceptionFactory()
        {
            ClassUnderTest
                .Invoking(async c => await c.ValidateAsync(invalidProfile))
                .Should().Throw<ValidationException>().Where(e => e == validationException);
        }

    }

    #endregion
}