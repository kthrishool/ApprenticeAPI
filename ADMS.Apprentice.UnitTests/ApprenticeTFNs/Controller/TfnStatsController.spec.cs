using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages.TFN;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Services;
using Adms.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ADMS.Apprentice.Api.Controllers.Tfn;
using Adms.Shared.Database;
using Adms.Shared.Paging;
using Adms.Shared.Testing;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ADMS.Apprentice.UnitTests.ApprenticeTFNs.Controller
{
    #region WhenGettingTFNStats
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithoutProfileUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = null;

        protected override void Given()
        {
            paging = new PagingInfo
            {
                Page = 1,

            };

            tfn = new ApprenticeTFN
            {
                Id = 1,
                ApprenticeId = 1,
                TaxFileNumber = "123456789",
                Profile = null,
                StatusCode = TFNStatus.NOCH,
                CreatedOn = DateTime.Today,
                StatusDate = DateTime.Today.AddDays(-2)
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
               .GetMock<IRepository>()
               .Setup(x => x.Retrieve<ApprenticeTFN>())
               .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(It.IsAny<IQueryable<ApprenticeTFN>>(), It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithNullProfile()
        {
            result.Should().NotBeNull();
            result.Result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
            pagedList.Results[0].NumberOfDaysSinceTheMismatch.Should().Be((DateTime.Today - tfn.StatusDate).TotalDays.ToString());
        }
    }

    #endregion


    #region WhenGettingTFNStatsWithProfile
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = null;

        protected override void Given()
        {
            paging = new PagingInfo
            {
                Page = 1,

            };

            tfn = new ApprenticeTFN
            {
                Id = 1,
                ApprenticeId = 1,
                TaxFileNumber = "123456789",
                Profile = new Profile
                {
                    FirstName = "first",
                    Surname = "last",
                    BirthDate = DateTime.Today,

                },
                StatusCode = TFNStatus.NOCH,
                StatusDate = DateTime.Today.AddDays(1),
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(It.IsAny<IQueryable<ApprenticeTFN>>(), It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithProfile()
        {
            result.Should().NotBeNull();
            result.Result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
            pagedList.Results[0].NumberOfDaysSinceTheMismatch.Should().Be("<1");
        }
    }

    #endregion

    #region WhenGettingTFNStatsWithFilterOnFirstName
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "first" };

        protected override void Given()
        {

            paging = new PagingInfo
            {
                Page = 1,

            };

            tfn = new ApprenticeTFN
            {
                Id = 1,
                ApprenticeId = 1,
                TaxFileNumber = "123456789",
                Profile = new Profile
                {
                    Surname = "surname",
                    FirstName = "firstname",
                    BirthDate = DateTime.Today
                },
                StatusCode = TFNStatus.MTCH,
                StatusDate = DateTime.Today,
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(It.IsAny<IQueryable<ApprenticeTFN>>(), It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
            pagedList.Results[0].NumberOfDaysSinceTheMismatch.Should().Be("-");
        }
    }
    #endregion

    #region WhenGettingTFNStatsWithFilterOnSurname
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithSurnameFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "surname" };

        protected override void Given()
        {

            paging = new PagingInfo
            {
                Page = 1,

            };

            tfn = new ApprenticeTFN
            {
                Id = 1,
                ApprenticeId = 1,
                TaxFileNumber = "123456789",
                Profile = new Profile
                {
                    Surname = "surname",
                    FirstName = "firstname",
                    BirthDate = DateTime.Today
                },
                StatusCode = TFNStatus.NOCH,
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(It.IsAny<IQueryable<ApprenticeTFN>>(), It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithSurnameFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
        }
    }
    #endregion

    #region WhenGettingTFNStatsWithFilterOnSurname
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithDobFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria()
        {
            Keyword = DateTime.Today.ToString("d/M/yyyy")
        };

        protected override void Given()
        {

            paging = new PagingInfo
            {
                Page = 1,

            };

            tfn = new ApprenticeTFN
            {
                Id = 1,
                ApprenticeId = 1,
                TaxFileNumber = "123456789",
                Profile = new Profile
                {
                    Surname = "surname",
                    FirstName = "firstname",
                    BirthDate = DateTime.Today
                },
                StatusCode = TFNStatus.NOCH,
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(It.IsAny<IQueryable<ApprenticeTFN>>(), It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithDobFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
        }
    }
    #endregion
}