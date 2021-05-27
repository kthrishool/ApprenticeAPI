﻿using System.Collections.Generic;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Exceptions;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
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
            ClassUnderTest.Validate(profile);
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
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidUSI))
                .Returns(validationException);

            ClassUnderTest
                .Invoking(c => c.Validate(profile))
                .Should().Throw<ValidationException>();
        }

        private void RunPositiveUSITest(Profile profile)
        {
            ClassUnderTest.Invoking(c => c.Validate(profile))
                .Should().NotThrow<ValidationException>();
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
            ThrowExceptionForUsiTest(new Profile
            {
                USIs = new List<ApprenticeUSI>()
                {
                    new ApprenticeUSI()
                    {
                        USI = "", ActiveFlag = true, USIStatus = "test"
                    }
                }
            });
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void DoNothingIfUSIIsValid()
        {
            RunPositiveUSITest(new Profile
            {
                USIs = new List<ApprenticeUSI>()
                {
                    new ApprenticeUSI()
                    {
                        USI = "23456789D1", ActiveFlag = true, USIStatus = "test"
                    }
                }
            });
        }


        /// <summary>
        /// </summary>
        [TestMethod]
        public void ThrowExceptionWhenUSIIsInvalid()
        {
            RunPositiveUSITest(new Profile
            {
                USIs = new List<ApprenticeUSI>()
                {
                    new ApprenticeUSI()
                    {
                        USI = "23456789D4", ActiveFlag = true, USIStatus = "test"
                    }
                }
            });
        }
    }

    #endregion
}