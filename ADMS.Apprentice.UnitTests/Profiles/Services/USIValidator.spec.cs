using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adms.Claims.UnitTests.ClaimApplications.Entities
{
    #region WhenValidatingAUSIValidator

    [TestClass]
    public class WhenValidatingAUSIValidator : GivenWhenThen<USIValidator>
    {
        //  private AaipClaimApplication application;
        private Profile profile;
        private ApprenticeUSI apprenticeUSI;
        private ValidationException validationException;


        protected override void Given()
        {
            //  application = new AaipClaimApplication();

            profile = new Profile();
            apprenticeUSI = new ApprenticeUSI()
            {
                USI = "test",
                ActiveFlag = true

            };
            profile.USIs.Add(apprenticeUSI);
            validationException = new ValidationException(null, (ValidationError) null);
        }

        [TestMethod]
        public void IsFalseIfNoClaimTypes()
        {
            ClassUnderTest.Validate(profile);
        }


        [TestMethod]
        public void DoesNothingIfUSIIsNull()
        {
            profile = new Profile();
            ClassUnderTest.Invoking(c => c.Validate(profile))
                .Should().NotThrow<ValidationException>();
        }

        /// <summary>
        /// Insert a profile record and check if the email has been updated .
        /// </summary>
        [TestMethod]
        public void ThrowsValidationExceptionIfUSIIsNull()
        {
            profile = new Profile();

            profile.USIs = new List<ApprenticeUSI>() {new ApprenticeUSI() {USI = "", ActiveFlag = true, USIStatus = "test"}};

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidUSI))
                .Returns(validationException);

            ClassUnderTest
                .Invoking(c => c.Validate(profile))
                .Should().Throw<ValidationException>();
        }
    }

    #endregion
}