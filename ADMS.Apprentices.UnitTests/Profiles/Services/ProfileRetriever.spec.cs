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
using Adms.Shared.Paging;
using Moq;

namespace ADMS.Apprentices.UnitTests.Profiles.Services
{
    #region WhenRetrievingProfilesWithSearchOptions

    [TestClass]
    public class WhenRetrievingProfilesWithSearchOptions : GivenWhenThen<ProfileRetreiver>
    {
        PagedList<ProfileListModel> profiles;
        private ProfileSearchMessage message;
        private PagingInfo paging;        
        private ICollection<ProfileSearchResultModel> searchResults = new List<ProfileSearchResultModel>();        

        protected override void Given()
        {            
            message = new ProfileSearchMessage
            {
                Name = ProfileConstants.Surname,
                BirthDate = ProfileConstants.Birthdate
            };
            searchResults.Add(new ProfileSearchResultModel(
                123, ProfileConstants.Profiletype, ProfileConstants.Firstname,
                ProfileConstants.Surname, ProfileConstants.Secondname,
                ProfileConstants.Birthdate, ProfileConstants.Emailaddress,
                ProfileConstants.USI, null, null, null, 20));

            paging = new PagingInfo
            {
                Page = 1,

            };
            
            Container.GetMock<IApprenticeRepository>()
                .Setup(r => r.GetProfilesAsync(It.IsAny<ProfileSearchMessage>()))
                .ReturnsAsync(() => searchResults);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedInMemoryList(searchResults, It.IsAny<PagingInfo>()))
                .Returns(new PagedInMemoryList<ProfileSearchResultModel>(paging, searchResults));
        }

        protected override async void When()
        {
            profiles = await ClassUnderTest.RetreiveList(paging, message);
        }

        [TestMethod]
        public void ShouldReturnProfiles()
        {
            profiles.Should().NotBeNull();
            profiles.TotalItems.Should().Be(1);
        }
    }
    #endregion

    #region WhenRetrievingProfilesWithNoSearchOptions

    [TestClass]
    public class WhenRetrievingProfilesWithNoSearchOptions : GivenWhenThen<ProfileRetreiver>
    {
        PagedList<ProfileListModel> profiles;
        private ProfileSearchMessage message;
        private PagingInfo paging;
        private Profile profile;        

        protected override void Given()
        {
            profile = new Profile { Id = 1, ActiveFlag = true };
            message = new ProfileSearchMessage();
            
            var results = new EnumerableQuery<Profile>(new List<Profile> { profile });
            paging = new PagingInfo
            {
                Page = 1,

            };
            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<Profile>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedListAsync(results, It.IsAny<PagingInfo>()))
                .ReturnsAsync(new PagedList<Profile>(paging, results.Count(), false, results));            
        }

        protected override async void When()
        {
            profiles = await ClassUnderTest.RetreiveList(paging, message);
        }

        [TestMethod]
        public void ShouldReturnProfiles()
        {
            profiles.Should().NotBeNull();
            profiles.TotalItems.Should().Be(1);
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

        protected override async void When()
        {
            searchResults = await ClassUnderTest.Search(message);
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
    