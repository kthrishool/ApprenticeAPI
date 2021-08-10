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
using System;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenValidatingAUSIValidator

    [TestClass]
    public class WhenValidatingAUSIValidator : GivenWhenThen<USIValidator>
    {
        private Profile profile;
        private ApprenticeUSI apprenticeUSI;
       
        protected override void Given()
        {
            profile = new Profile();
            apprenticeUSI = new ApprenticeUSI()
            {
                USI = "test",
                ActiveFlag = true
            };
            profile.USIs.Add(apprenticeUSI);
            
        }

        [TestMethod]
        public void ThrowsExceptionIfUSIIsNullAndNoExemptionReason()
        {            
            profile = new Profile();
            ClassUnderTest.Invoking(c => ( c.Validate(profile)).HasExceptions().Should().BeTrue());
            ClassUnderTest
                .Invoking(c => c.Validate(profile).ThrowAnyExceptions())
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void NoExceptionIfUSIIsNullAndExemptionReasonProvided()
        {
            profile = new Profile();
            profile.NotPovidingUSIReasonCode = "NOUSI";
            ClassUnderTest.Invoking(c => (c.Validate(profile)).HasExceptions().Should().BeFalse());
            ClassUnderTest
                .Invoking(c => c.Validate(profile).ThrowAnyExceptions())
                .Should().NotThrow<AdmsValidationException>();
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