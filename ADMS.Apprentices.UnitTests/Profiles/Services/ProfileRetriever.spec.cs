﻿using System.Linq;
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

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenRetrievingProfiles

    [TestClass]
    public class WhenRetrievingProfiles : GivenWhenThen<ProfileRetreiver>
    {
        IQueryable<Profile> profiles;

        protected override void When()
        {
            profiles = ClassUnderTest.RetreiveList();
        }

        [TestMethod]
        public void ShouldReturnProfiles()
        {
            profiles.Should().NotBeNull();
        }
    }
    #endregion

    #region WhenSearchingProfiles

    [TestClass]
    public class WhenSearchingProfiles : GivenWhenThen<ProfileRetreiver>
    {
        ICollection<ProfileSearchResultModel> searchResults;
        private ProfileSearchMessage message;

        protected override void Given()
        {
            message = new ProfileSearchMessage
            {
                Name = ProfileConstants.Surname,                
                BirthDate = ProfileConstants.Birthdate
            };
        }

        protected override void When()
        {
            searchResults = ClassUnderTest.Search(message);
        }

        [TestMethod]
        public void ShouldFetchProfilesFromRepository()
        {
            Container.GetMock<IApprenticeRepository>().Verify(r => r.GetProfilesAsync(message));
        }

        [TestMethod]
        public void ThrowErrorWhenPhonenumberIsLessNumbers()
        {            
            message = new ProfileSearchMessage
            {
                Phonenumber = "1234567"
            };

            ClassUnderTest.Invoking(c => c.Search(message))
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowErrorWhenEmailIsLessCharacters()
        {
            message = new ProfileSearchMessage
            {
                EmailAddress = "abc"
            };

            ClassUnderTest.Invoking(c => c.Search(message))
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowErrorWhenNoSearchParamsGiven()
        {
            message = new ProfileSearchMessage();

            ClassUnderTest.Invoking(c => c.Search(message))
                .Should().Throw<AdmsValidationException>();
        }

        [TestMethod]
        public void ThrowErrorWhenSearchByStateAndNoOtherParamsGiven()
        {
            message = new ProfileSearchMessage()
            {
                Address = "act"
            };

            ClassUnderTest.Invoking(c => c.Search(message))
                .Should().Throw<AdmsValidationException>();
        }

    }
    #endregion
}
    