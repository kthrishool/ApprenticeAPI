using System;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Messages;
using ADMS.Apprentice.Core.Services;
using Adms.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADMS.Apprentice.Core.Models;
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

namespace ADMS.Apprentice.UnitTests.ApprenticeTFNs.Services
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
    public class WhenRetreivingAApprenticeTFN : GivenWhenThen<ApprenticeTFNRetreiver>
    {
        private ApprenticeTFNModel tfnDetail;
        private ApprenticeTFNV1 message;
        private const int apprenticeId = 111;


        internal static Mock<DbSet<ApprenticeTFN>> SingleApprenticeTFN()
        {
            var ApprenticeTfn3 = new ApprenticeTFN
            {
                ApprenticeId = apprenticeId + 2,
                TaxFileNumber = "999888777",
                StatusCode = TFNStatus.New,
                StatusReasonCode = "New"
            };

           var data = new List<ApprenticeTFN> {
                    new ApprenticeTFN
                    {
                        ApprenticeId = apprenticeId,
                        TaxFileNumber = "123456789",
                        StatusCode = TFNStatus.New,
                        StatusReasonCode = "New"
                    },
                new ApprenticeTFN
                {
                    ApprenticeId = apprenticeId + 1,
                    TaxFileNumber = "999888777",
                    StatusCode = TFNStatus.New,
                    StatusReasonCode = "New"
                }
            };

            IQueryable<ApprenticeTFN> caList = new List<ApprenticeTFN>(data).AsQueryable();

            var mockDbSet = new Mock<DbSet<ApprenticeTFN>>();

            mockDbSet.As<IQueryable<ApprenticeTFN>>()
                .Setup(x => x.GetEnumerator())
                .Returns(caList.GetEnumerator());

            mockDbSet.As<IQueryable<ApprenticeTFN>>()
                .Setup(x => x.Provider)
                .Returns(new AsyncQueryProvider<ApprenticeTFN>(caList.Provider));

            mockDbSet.As<IQueryable<ApprenticeTFN>>()
                .Setup(x => x.Expression)
                .Returns(caList.Expression);

            CancellationToken cancellationToken = default;

            return mockDbSet;
        }


        protected override void Given()
        {


        }

        protected override void When()
        {
            Mock<DbSet<ApprenticeTFN>> mockDbSet = SingleApprenticeTFN();

            var mockCtx = new Mock<IRepository>();
           // mockCtx.SetupGet(c => c.Retrieve<ApprenticeTFN>).Returns(mockDbSet.Object);

            tfnDetail = ClassUnderTest.Get(apprenticeId).Result;
        }

        [TestMethod]
        public async Task ShouldReturnApprenticeTFN()
        {
            tfnDetail = await ClassUnderTest.Get(apprenticeId);

            tfnDetail.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldDecryptTheTFNn()
        {
            Container.GetMock<ICryptography>().Verify(r => r.DecryptTFN(message.ApprenticeId.ToString(), message.TaxFileNumber.ToString()));
        }

        [TestMethod]
        public void ShouldSetTheApprenticeId()
        {
            tfnDetail.ApprenticeId.Should().Be(message.ApprenticeId);
        }

        [TestMethod]
        public void ShouldSetDefaultValues()
        {
            tfnDetail.Status.Should().Be(TFNStatus.New);
        }

    }

    #endregion
}