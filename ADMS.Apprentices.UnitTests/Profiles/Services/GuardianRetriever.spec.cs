using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.Services.Validators;
using ADMS.Apprentices.UnitTests.Constants;
using Adms.Shared;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentices.Core.Models;
using System.Collections.Generic;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Adms.Shared.Exceptions;
using ADMS.Apprentices.Core.Exceptions;
using System;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenRetrievingAGuardian

    [TestClass]
    public class WhenRetrievingGuardian : GivenWhenThen<GuardianRetriever>
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
    public class WhenGuardianIsNull : GivenWhenThen<GuardianRetriever>
    {
        int apprenticeId;
        Profile profile;

        protected override void Given()
        {
            apprenticeId = 1;
            profile = new Profile
            {
                Id = apprenticeId,
                Surname = ProfileConstants.Surname,
                BirthDate = ProfileConstants.Birthdate,
                Guardian = null
            };
            Container
                .GetMock<IRepository>()
                .Setup(r => r.GetAsync<Profile>(apprenticeId, true))
                .Returns(Task.FromResult(profile));
        }

        [TestMethod]
        public void ShouldThrowException()
        {
            ClassUnderTest
                 .Invoking(async c => await c.GetAsync(apprenticeId))
                 .Should().Throw<AdmsNotFoundException>();
        }
    }
    #endregion
}
