using System;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;
using ADMS.Apprentices.Core.Services;
using ADMS.Services.Infrastructure.Core.Interface;
using Adms.Shared.Database;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IRepository = Adms.Shared.IRepository;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ADMS.Services.Infrastructure.Core.Exceptions;
using Adms.Shared.Exceptions;
using Adms.Shared.Services;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Services
{
    #region WhenSettingTfnForRevalidate
    [TestClass]
    public class WhenSettingTfnForRevalidate : TfNUpdaterBase
    {        
        protected override void Given()
        {
            currentDate = DateTime.Now;
            var mockDbSet = SingleApprenticeTFN();

            message = new ApprenticeTFNV1
            {
                ApprenticeId = 1,
                TaxFileNumber = 123456789
            };

            var mockIRepository = Container.GetMock<IRepository>();
            mockIRepository.Setup(x => x.Retrieve<ApprenticeTFN>()).Returns(mockDbSet.Object);

            var mockIContext = Container.GetMock<IContext>();
            mockIContext.Setup(x => x.DateTimeContext).Returns(currentDate);

            Container.GetMock<IContextRetriever>().Setup(x => x.GetContext()).Returns(mockIContext.Object);
        }

        protected override void When()
        {
            tfnDetail = ClassUnderTest.SetRevalidate(apprenticeId).Result;
        }

        [TestMethod]
        public void ShouldReturnApprenticeTFN()
        {
            tfnDetail.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldSetTheApprenticeId()
        {
            tfnDetail.ApprenticeId.Should().Be(apprenticeId);
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            tfnDetail.StatusCode.Should().Be(TFNStatus.TBVE);
            tfnDetail.StatusDate.Should().BeOnOrBefore(currentDate);
        }

        [TestMethod]
        public void ShouldSetNewCorrelationId()
        {
            tfnDetail.MessageQueueCorrelationId.Should().NotBe(guid1);
        }

    }

    #endregion

    #region WhenSettingTfnForUpdate
    [TestClass]
    public class WhenSettingTfnForUpdate : TfNUpdaterBase
    {
        protected override void Given()
        {
            currentDate = DateTime.Now;
            var mockDbSet = SingleApprenticeTFN();

            message = new ApprenticeTFNV1
            {
                ApprenticeId = apprenticeId,
                TaxFileNumber = 123456789
            };

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(mockDbSet.Object);

            Container
                .GetMock<ICryptography>()
                .Setup(x => x.EncryptTFN(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("*&^%$#");

            var mockIContext = Container.GetMock<IContext>();
            mockIContext.Setup(x => x.DateTimeContext).Returns(currentDate);

            Container.GetMock<IContextRetriever>().Setup(x => x.GetContext()).Returns(mockIContext.Object);
        }

        protected override void When()
        {
            tfnDetail = ClassUnderTest.Update(message).Result;
        }

        [TestMethod]
        public void ShouldReturnApprenticeTFN()
        {
            tfnDetail.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldSetTheApprenticeId()
        {
            tfnDetail.ApprenticeId.Should().Be(apprenticeId);
        }

        [TestMethod]
        public void ShouldSetTheTaxFileNumber()
        {
            tfnDetail.TaxFileNumber.Should().Be("*&^%$#");
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            tfnDetail.StatusCode.Should().Be(TFNStatus.TBVE);
            tfnDetail.StatusDate.Should().BeOnOrBefore(currentDate);
            tfnDetail.StatusReasonCode.Should().Be(null);
        }

        [TestMethod]
        public void ShouldSetNewCorrelationId()
        {
            tfnDetail.MessageQueueCorrelationId.Should().NotBe(guid1);
        }
    }

    #endregion

    #region WhenGetTfnForUpdateErrors

    [TestClass]
    public class WhenGetTfnForUpdateErrors : TfNUpdaterBase
    {
        protected override void Given()
        {
            currentDate = DateTime.Now;
            var mockDbSet = SingleApprenticeTFN();

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(mockDbSet.Object);

            var mockIContext = Container
                .GetMock<IContext>();

            mockIContext
                .Setup(x => x.DateTimeContext).Returns(currentDate);

            Container
                .GetMock<IContextRetriever>()
                .Setup(x => x.GetContext())
                .Returns(mockIContext.Object);
        }

        [TestMethod]
        public void ShouldThrowNotFoundException()
        {
            // NotFoundException validationException = new(null, "TaxFileNumber", "99");

            ClassUnderTest
                .Invoking(c => c.Get(99))
                .Should().Throw<AdmsNotFoundException>();

        }
    }


    #endregion

    #region TfNUpdaterBase
    public class TfNUpdaterBase : GivenWhenThen<ApprenticeTFNUpdater>
    {
        protected ApprenticeTFN tfnLookup;
        protected ApprenticeTFN tfnDetail;
        protected ApprenticeTFNV1 message;
        protected DateTime currentDate;
        protected const int apprenticeId = 111;

        protected static Guid guid1 = new();
        protected static Guid guid2 = new();
        protected static Guid guid3 = new();

        internal static Mock<DbSet<T>> GetMockDbSet<T>(ICollection<T> entities) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.AsQueryable().Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.AsQueryable().Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.AsQueryable().GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(entities.Add);
            return mockSet;
        }

        internal static Mock<DbSet<ApprenticeTFN>> SingleApprenticeTFN()
        {

            var data = new List<ApprenticeTFN> {
                new ApprenticeTFN
                {
                    ApprenticeId = apprenticeId,
                    TaxFileNumber = "123456789",
                    StatusCode = TFNStatus.NOCH,
                    StatusReasonCode = "No Match",
                    MessageQueueCorrelationId = guid1
                },
                new ApprenticeTFN
                {
                    ApprenticeId = apprenticeId + 1,
                    TaxFileNumber = "999888777",
                    StatusCode = TFNStatus.TBVE,
                    MessageQueueCorrelationId = guid2
                },
                new ApprenticeTFN
                {
                    ApprenticeId = apprenticeId + 2,
                    TaxFileNumber = "999888777",
                    StatusCode = TFNStatus.NOCH,
                    StatusReasonCode = "No Match",
                    MessageQueueCorrelationId = guid3
                }
            };

            var mockDbSet = GetMockDbSet<ApprenticeTFN>(data);
            return mockDbSet;
        }
    }

    #endregion
}