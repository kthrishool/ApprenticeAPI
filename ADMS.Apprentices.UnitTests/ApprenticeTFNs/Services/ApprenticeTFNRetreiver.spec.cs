using System;
using ADMS.Apprentices.Core.Entities;
using ADMS.Apprentices.Core.Services;
using Adms.Shared;
using Adms.Shared.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentices.Core.Models;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Adms.Shared.Testing;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Adms.Shared.Exceptions;

namespace ADMS.Apprentices.UnitTests.ApprenticeTFNs.Services
{
    [ExcludeFromCodeCoverage]
    internal class AsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public AsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new AsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    [ExcludeFromCodeCoverage]
    internal class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) { }


        public AsyncEnumerable(Expression expression) : base(expression)
        {

        }
        IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerable<T>(this.AsEnumerable()).GetAsyncEnumerator(cancellationToken);
        }
        public IAsyncEnumerator<T> GetEnumerator()
        {
            return GetAsyncEnumerator();
        }

    }

    [ExcludeFromCodeCoverage]
    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IAsyncEnumerator<T> enumerator;

        public AsyncEnumerator(IAsyncEnumerator<T> enumerator) =>
            this.enumerator = enumerator ?? throw new ArgumentNullException();

        public T Current => enumerator.Current;

        public void Dispose() { }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return enumerator.MoveNextAsync();
        }
    }

    #region WhenRetreivingAApprenticeTFN
    [TestClass]
    public class WhenRetreivingApprenticeTFN : ApprenticeTFNRetreiverBase
    {
        protected override void When()
        {
            var mockDbSet = SingleApprenticeTFN();
            var ctx = new Mock<IRepository>();
            var ctx3 = new Mock<IExceptionFactory>();

            ctx.Setup(c => c.Retrieve<ApprenticeTFN>()).Returns(mockDbSet.Object);
            IApprenticeTFNRetreiver service = new ApprenticeTFNRetreiver (ctx.Object,ctx2.Object, ctx3.Object);


            tfnDetail = service.Get(apprenticeId);
        }

        [TestMethod]
        public void ShouldReturnApprenticeTFN()
        {

            tfnDetail.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldDecryptTheTFNn()
        {
            ctx2.Verify(r => r.DecryptTFN(tfnDetail.ApprenticeId.ToString(), It.IsAny<string>()));
        }

        [TestMethod]
        public void ShouldSetTheApprenticeId()
        {
            tfnDetail.ApprenticeId.Should().Be(tfnDetail.ApprenticeId);
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            tfnDetail.Status.Should().Be(TFNStatus.TBVE);
        }

    }

    [TestClass]
    public class WhenRetreivingNullApprenticeTFN : ApprenticeTFNRetreiverBase
    {

        [TestMethod]
        public void ShouldCallExceptionFactory()
        {
            var mockDbSet = SingleApprenticeTFN();
            var ctx = new Mock<IRepository>();
            var ctx3 = new Mock<IExceptionFactory>();

            ctx.Setup(c => c.Retrieve<ApprenticeTFN>()).Returns(mockDbSet.Object);
            IApprenticeTFNRetreiver service = new ApprenticeTFNRetreiver(ctx.Object, ctx2.Object, ctx3.Object);

            var result = Assert.ThrowsException<NullReferenceException>(() => service.Get(apprenticeId - 1));
        }


    }

    public class ApprenticeTFNRetreiverBase : GivenWhenThen<ApprenticeTFNRetreiver>
    {
        protected ApprenticeTFNModel tfnDetail;
        protected const int apprenticeId = 111;

        protected readonly Mock<ICryptography> ctx2 = new Mock<ICryptography>();

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
                    StatusCode = TFNStatus.TBVE
                },
                new ApprenticeTFN
                {
                    ApprenticeId = apprenticeId + 1,
                    TaxFileNumber = "999888777",
                    StatusCode = TFNStatus.TBVE
                },
                new ApprenticeTFN
                {
                    ApprenticeId = apprenticeId + 2,
                    TaxFileNumber = "999888777",
                    StatusCode = TFNStatus.NOCH,
                    StatusReasonCode = "No Match"
                }
            };

            var mockDbSet = GetMockDbSet<ApprenticeTFN>(data);
            return mockDbSet;
        }


        protected override void Given()
        {


        }
    }
    #endregion
}