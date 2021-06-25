using System;
using System.Linq;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Messages.TFN;
using ADMS.Apprentices.Core.Services;
using ADMS.Services.Infrastructure.Core.Interface;
using Adms.Shared.Database;
using Adms.Shared.Services;
using Adms.Shared.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IRepository = Adms.Shared.IRepository;
using ADMS.Apprentices.Core.Exceptions;
using Adms.Shared.Exceptions;
using ADMS.Services.Infrastructure.Core.Exceptions;
using ADMS.Services.Infrastructure.Core.Validation;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Services
{
    #region WhenCreatingApprenticeTFN
    [TestClass]
    public class WhenCreatingApprenticeTFN : GivenWhenThen<ApprenticeTFNCreator>
    {        
        private ApprenticeTFN tfnDetail;
        private ApprenticeTFNV1 message;
        private DateTime currentDate;

        protected override void Given()
        {
            currentDate = DateTime.Now;
            message = new ApprenticeTFNV1
            {
                ApprenticeId = 1,
                TaxFileNumber = 123456789
            };

            var mockIContext = Container
                .GetMock<IContext>();
            mockIContext
                .Setup(x => x.DateTimeContext)
                .Returns(currentDate);

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Get<Profile>(message.ApprenticeId))
                .Returns(new Profile());

            Container
                .GetMock<IContextRetriever>()
                .Setup(x => x.GetContext())
                .Returns(mockIContext.Object);
        }

        protected override void When()
        {
            tfnDetail = ClassUnderTest.CreateAsync(message).Result;
        }

        [TestMethod]
        public void ShouldReturnApprenticeTFN()
        {
            tfnDetail.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldAddTheApprenticeTFNToTheDatabase()
        {
            Container.GetMock<IRepository>().Verify(r => r.Insert(tfnDetail));
        }

        [TestMethod]
        public void ShouldEncryptTheTFNn()
        {
            Container.GetMock<ICryptography>().Verify(r => r.EncryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber.ToString()));
        }

        [TestMethod]
        public void ShouldSetTheApprenticeId()
        {
            tfnDetail.ApprenticeId.Should().Be(message.ApprenticeId);
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            tfnDetail.StatusCode.Should().Be(TFNStatus.TBVE);
            tfnDetail.StatusDate.Should().BeOnOrBefore(currentDate);
        }

        [TestMethod]
        public void ShouldSetNewGuidValues()
        {
            tfnDetail.MessageQueueCorrelationId.Should().NotBeEmpty();
        }

    }

    [TestClass]
    public class WhenCreatingApprenticeTFNErrors : TfNCreatorBase
    {
        protected override void Given()
        {
            currentDate = DateTime.Now;
            message = new ApprenticeTFNV1
            {
                ApprenticeId = 1,
                TaxFileNumber = 123456789
            };

            var mockIContext = Container
                .GetMock<IContext>();

            mockIContext
                .Setup(x => x.DateTimeContext)
                .Returns(currentDate);

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Get<Profile>(message.ApprenticeId))
                .Returns(new Profile());

            Container
                .GetMock<IContextRetriever>()
                .Setup(x => x.GetContext())
                .Returns(mockIContext.Object);
        }

        [TestMethod]
        public void ShouldThrowNotFoundError()
        {
            ValidationException validationException;
            validationException = new ValidationException(null, (ValidationError)null);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidApprenticeId))
                .Returns(validationException);


            message.ApprenticeId = -1;
            ClassUnderTest
                .Invoking(async c => await c.CreateAsync(message))
                .Should().Throw<ValidationException>().Where(e => e == validationException);

        }

        [TestMethod]
        public void ShouldThrowNoTfnFoundError()
        {
            ValidationException validationException;
            validationException = new ValidationException(null, (ValidationError)null);

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.InvalidTFN))
                .Returns(validationException);


            message.ApprenticeId = 1;
            message.TaxFileNumber = -1;
            ClassUnderTest
                .Invoking(async c => await c.CreateAsync(message))
                .Should().Throw<ValidationException>().Where(e => e == validationException);

        }

        [TestMethod]
        public void ShouldThrowNoProfileFoundError()
        {
            message.ApprenticeId = 11;
            message.TaxFileNumber = 11;

            NotFoundException validationException;
            validationException = new NotFoundException(null, "Apprentice Profile", "11");

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateNotFoundException("Apprentice Profile", "11"))
                .Returns(validationException);

            ClassUnderTest
                .Invoking(async c => await c.CreateAsync(message))
                .Should().Throw<NotFoundException>().Where(e => e == validationException);

        }

        [TestMethod]
        public void ShouldThrowTfnRecordFoundError()
        {
            ValidationException validationException;
            validationException = new ValidationException(null, (ValidationError)null);
            var mockDbSet = SingleApprenticeTFN();
            message.ApprenticeId = apprenticeId;
            message.TaxFileNumber = 1;

            Container
                .GetMock<IExceptionFactory>()
                .Setup(r => r.CreateValidationException(ValidationExceptionType.TFNAlreadyExists))
                .Returns(validationException);

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Get<Profile>(message.ApprenticeId))
                .Returns(new Profile());

            Container
                .GetMock<IRepository>()
                .Setup(x => x.Retrieve<ApprenticeTFN>())
                .Returns(mockDbSet.Object);

            ClassUnderTest
                .Invoking(async c => await c.CreateAsync(message))
                .Should().Throw<ValidationException>().Where(e => e == validationException);

        }

    }
    public class TfNCreatorBase : GivenWhenThen<ApprenticeTFNCreator>
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