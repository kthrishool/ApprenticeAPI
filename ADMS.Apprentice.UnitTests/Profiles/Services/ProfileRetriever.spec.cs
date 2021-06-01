﻿using System.Linq;
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

namespace ADMS.Apprentice.UnitTests.Profiles.Services
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
                Surname = ProfileConstants.Surname,
                FirstName = ProfileConstants.Firstname,
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
    }
    #endregion
}
    