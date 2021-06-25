using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;
using ADMS.Apprentices.Core.Services;
using Adms.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using ADMS.Apprentices.Api.Controllers.Tfn;
using Adms.Shared.Paging;
using Adms.Shared.Testing;
using Moq;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Controller
{
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
                .Setup(x => x.ToPagedList(results, It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

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
                .Setup(x => x.ToPagedList(results, It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

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
                .Setup(x => x.ToPagedList(results, It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

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
                .Setup(x => x.ToPagedList(results, It.IsAny<PagingInfo>()))
                .Returns(new PagedList<ApprenticeTFN>(paging, results.Count(), false, results));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

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

    #region WhenGettingTFNStatsWithFilterOnMatchedStatus
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithMatchedStatusFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "surname", StatusCode = "mtch"};

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

            var mtchTfn = new ApprenticeTFN
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
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, mtchTfn });
            var resultsWithStatus = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> {mtchTfn});

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(resultsWithStatus, It.IsAny<PagingInfo>())) 
                .Returns(new PagedList<ApprenticeTFN>(paging, resultsWithStatus.Count(), false, resultsWithStatus));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithMatchedTfnFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
            pagedList.Results.First().TfnVerificationStatus.Should().Be(TFNStatus.MTCH.ToString());
        }
    }
    #endregion

    #region WhenGettingTFNStatsWithFilterOnMatchedStatus
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithNotMatchedStatusFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "surname", StatusCode = "noch" };

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

            var mtchTfn = new ApprenticeTFN
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
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, mtchTfn });
            var resultsWithStatus = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(resultsWithStatus, It.IsAny<PagingInfo>())) 
                .Returns(new PagedList<ApprenticeTFN>(paging, resultsWithStatus.Count(), false, resultsWithStatus));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithNotMatchedTfnFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
            pagedList.Results.First().TfnVerificationStatus.Should().Be(TFNStatus.NOCH.ToString());
        }
    }
    #endregion

    #region WhenGettingTFNStatsWithFilterOnMatchedStatus
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithSubmittedStatusFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "surname", StatusCode = "SBMT" };

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
                StatusCode = TFNStatus.SBMT,
                CreatedOn = DateTime.Today
            };

            var mtchTfn = new ApprenticeTFN
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
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, mtchTfn });
            var resultsWithStatus = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(resultsWithStatus, It.IsAny<PagingInfo>())) 
                .Returns(new PagedList<ApprenticeTFN>(paging, resultsWithStatus.Count(), false, resultsWithStatus));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithSubmittedTfnFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
            pagedList.Results.First().TfnVerificationStatus.Should().Be(TFNStatus.SBMT.ToString());
        }
    }
    #endregion

    #region WhenGettingTFNStatsWithFilterOnMatchedStatus
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithErrorStatusFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "surname", StatusCode = "TERR" };

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
                StatusCode = TFNStatus.TERR,
                CreatedOn = DateTime.Today
            };

            var mtchTfn = new ApprenticeTFN
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
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, mtchTfn });
            var resultsWithStatus = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(resultsWithStatus, It.IsAny<PagingInfo>())) 
                .Returns(new PagedList<ApprenticeTFN>(paging, resultsWithStatus.Count(), false, resultsWithStatus));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));
        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithErrorStatusTfnFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
            pagedList.Results.First().TfnVerificationStatus.Should().Be(TFNStatus.TERR.ToString());
        }
    }
    #endregion

    #region WhenGettingTFNStatsWithFilterOnMatchedStatus
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithToBeVerifiedStatusFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "surname", StatusCode = "tbve" };

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
                StatusCode = TFNStatus.TBVE,
                CreatedOn = DateTime.Today
            };

            var mtchTfn = new ApprenticeTFN
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
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, mtchTfn });
            var resultsWithStatus = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(resultsWithStatus, It.IsAny<PagingInfo>())) 
                .Returns(new PagedList<ApprenticeTFN>(paging, resultsWithStatus.Count(), false, resultsWithStatus));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithToBeVerifiedTfnFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(1);
            pagedList.Results.First().TfnVerificationStatus.Should().Be(TFNStatus.TBVE.ToString());
        }
    }
    #endregion

    #region WhenGettingTFNStatsWithFilterOnInProgStatus
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithInProgStatusFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "surname", StatusCode = "inprog" };

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
                StatusCode = TFNStatus.TBVE,
                CreatedOn = DateTime.Today
            };

            var mtchTfn = new ApprenticeTFN
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
                CreatedOn = DateTime.Today
            };

            var sbmtTfn = new ApprenticeTFN
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
                StatusCode = TFNStatus.SBMT,
                CreatedOn = DateTime.Today
            };
            

            var noMtchTfn = new ApprenticeTFN
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

            var errorTfn = new ApprenticeTFN
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
                StatusCode = TFNStatus.TERR,
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, mtchTfn, noMtchTfn, sbmtTfn, errorTfn });
            var resultsWithStatus = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, sbmtTfn, errorTfn }); // The sequence in which the records are added is important because this object is used in mocking the method and if the elements are not in the same sequence as the parent list (results <<EnumerableQuery>>) then the mocking will not happen.

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(resultsWithStatus, It.IsAny<PagingInfo>())) 
                .Returns(new PagedList<ApprenticeTFN>(paging, resultsWithStatus.Count(), false, resultsWithStatus));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithInProgStatusFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(3);
            // If mocking works with the given list (resultsWithStatus), we have ensured that the records returned have one of these statuses: TERR, SBMT and TBVE.
        }
    }
    #endregion

    #region WhenGettingTFNStatsWithFilterOnInvalidStatus
    [TestClass, ExcludeFromCodeCoverage]
    public class WhenGettingApprenticeTfnWithInvalidStatusFilterUsingTheApi : GivenWhenThen<TFNStatsController>
    {
        private ApprenticeTFN tfn;
        private ActionResult<PagedList<TFNStatsV1>> result;
        private PagingInfo paging;
        private TFNStatsCriteria criteria = new TFNStatsCriteria() { Keyword = "surname", StatusCode = "tbvesss" };

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
                StatusCode = TFNStatus.TBVE,
                CreatedOn = DateTime.Today
            };

            var mtchTfn = new ApprenticeTFN
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
                CreatedOn = DateTime.Today
            };

            var results = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, mtchTfn });
            var resultsWithStatus = new EnumerableQuery<ApprenticeTFN>(new List<ApprenticeTFN> { tfn, mtchTfn });

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(results);

            Container.GetMock<IPagingHelper>()
                .Setup(x => x.ToPagedList(resultsWithStatus, It.IsAny<PagingInfo>())) 
                .Returns(new PagedList<ApprenticeTFN>(paging, resultsWithStatus.Count(), false, resultsWithStatus));

            Container.GetMock<ITFNStatsRetriever>().Setup(x => x.RetrieveTfnStats(criteria)).Returns(new TFNStatsRetriever(Container.GetMock<IRepository>().Object)
                .RetrieveTfnStats(criteria));

        }

        protected override void When()
        {
            result = ClassUnderTest.List(paging, criteria);
        }

        [TestMethod]
        public void ReturnsStatsForGivenPageWithInvalidStatusFilter()
        {
            result.Should().NotBeNull();
            var list = (Microsoft.AspNetCore.Mvc.OkObjectResult)result.Result;
            list.Should().NotBeNull();
            list.Value.Should().NotBeNull();
            var pagedList = (PagedList<TFNStatsV1>)list.Value;
            pagedList.Should().NotBeNull();
            pagedList.Results.Should().NotBeNull();
            pagedList.Results.Should().HaveCount(2);
        }
    }
    #endregion

}