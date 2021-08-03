using System.Collections.Generic;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Exceptions;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingAUSIValidator

    [TestClass]
    public class WhenValidatingAUSIValidator : GivenWhenThen<USIValidator>
    {
        private Profile profile;
        private ApprenticeUSI apprenticeUSI;
        private ValidationException validationException;


        protected override void Given()
        {
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
        public void NoExceptionIfUSIIsNull()
        {
            profile = new Profile();
            ClassUnderTest.Validate(profile).ThrowAnyExceptions();
        }

        [TestMethod]
        public void ThrowExceptionWhenUSIIsLessthan10Char()
        {
            profile = new Profile();
            apprenticeUSI = new ApprenticeUSI()
            {
                USI = "test",
                ActiveFlag = true
            };
            profile.USIs.Add(apprenticeUSI);
            RunNegativeUSIText(profile);
        }

        private void RunNegativeUSIText(Profile profile)
        {
            ClassUnderTest
                .Invoking(c => c.Validate(profile).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        private void RunPositiveUSITest(Profile profile)
        {
            ClassUnderTest.Invoking(c => c.Validate(profile).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
        }

        private void ThrowExceptionForUsiTest(Profile profile)
        {
            RunNegativeUSIText(profile);
        }

        [TestMethod]
        public void DoesNothingIfUSIIsNull()
        {
            RunPositiveUSITest(new Profile());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void ThrowsValidationExceptionIfUSIIsNull()
        {
            profile = new Profile();
            apprenticeUSI = new ApprenticeUSI()
            {
                USI = "",
                ActiveFlag = true,
                USIStatus = "test"
            };
            profile.USIs.Add(apprenticeUSI);

            ThrowExceptionForUsiTest(profile);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void DoNothingIfUSIIsValid()
        {
            profile = new Profile();
            apprenticeUSI = new ApprenticeUSI()
            {
                USI = "147852369Q",
                ActiveFlag = true,
                USIStatus = "test"
            };
            profile.USIs.Add(apprenticeUSI);

            RunPositiveUSITest(profile);
        }


        /// <summary>
        /// </summary>
        [TestMethod]
        public void ThrowExceptionWhenUSIIsInvalid()
        {
            profile = new Profile();
            apprenticeUSI = new ApprenticeUSI()
            {
                USI = "147852369I",
                ActiveFlag = true,
                USIStatus = "test"
            };
            profile.USIs.Add(apprenticeUSI);

            ThrowExceptionForUsiTest(profile);
        }
    }

    #endregion
}