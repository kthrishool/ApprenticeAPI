using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.Core.Services.Validators;
using ADMS.Apprentice.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentice.Core.Models;
using System.Collections.Generic;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using ADMS.Apprentice.Core.Exceptions;
using System;

namespace ADMS.Apprentice.UnitTests.Profiles.Services
{
    #region WhenRetrievingAGuardian

    [TestClass]
    public class WhenRetrievingGuardian : GivenWhenThen<GuardianRetreiver>
    {
        int apprenticeId;
        Profile profile;
        Guardian guardian;

        protected override void Given()
        {
            apprenticeId = 1;            
            profile = new Profile
            {
                Id = apprenticeId,
                Surname = ProfileConstants.Surname,
                BirthDate = ProfileConstants.Birthdate,
                Guardian = new Guardian
                {
                    Id = 1,
                    ApprenticeId = apprenticeId,
                }
            };
            Container
                .GetMock<IRepository>()
                .Setup(r => r.GetAsync<Profile>(apprenticeId, true))
                .Returns(Task.FromResult(profile));
        }

        protected override async void When()
        {
            guardian = await ClassUnderTest.GetAsync(apprenticeId);
        }

        [TestMethod]
        public void ShouldReturnGuardian()
        {
            guardian.Should().NotBeNull();
        }       
    }
    #endregion

    #region WhenGuardianIsNull

    [TestClass]
    public class WhenGuardianIsNull : GivenWhenThen<GuardianRetreiver>
    {
        int apprenticeId;
        Profile profile;
        Guardian guardian;
        NotFoundException exception;

        protected override void Given()
        {
            apprenticeId = 1;
            guardian = new Guardian
            {
                Id = 1,
                ApprenticeId = apprenticeId,
            };
            profile = new Profile
            {
                Id = apprenticeId,
                Surname = ProfileConstants.Surname,
                BirthDate = ProfileConstants.Birthdate,
                Guardian = null
            };
            exception = new NotFoundException(null, "Apprentice guardian", $"ApprenticeId { profile.Id }");
            Container
                .GetMock<IRepository>()
                .Setup(r => r.GetAsync<Profile>(apprenticeId, true))
                .Returns(Task.FromResult(profile));
            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateNotFoundException("Apprentice guardian", $"ApprenticeId {profile.Id}")).Returns(exception);
        }

        [TestMethod]
        public void ShouldThrowException()
        {
            ClassUnderTest
                 .Invoking(async c => await c.GetAsync(apprenticeId))
                 .Should().Throw<NotFoundException>();
        }
    }
    #endregion
}
