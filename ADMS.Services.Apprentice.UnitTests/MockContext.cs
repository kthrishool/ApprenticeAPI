using ADMS.Services.Infrastructure.Core.Data;
using ADMS.Services.Infrastructure.Core.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AMDS.Services.Apprentice.UnitTests
{
    [ExcludeFromCodeCoverage]
    internal class MockContext : IContext
    {
        public bool DebugCurrentUser
        {
            get { return false; }
        }
    

        public bool IsTransactionInProgress {
            get { return false; }
        }

        public bool IsDistributedTransaction {
            get { return false; }
        }

        public DateTime DateTimeContext => throw new NotImplementedException();

        public string UniqueRequestMessageId => throw new NotImplementedException();

        public Guid? EventMessageId => throw new NotImplementedException();

        public IUser User => throw new NotImplementedException();

        public Guid CorrelationId => throw new NotImplementedException();

        public Guid? ChainId => throw new NotImplementedException();

        public Guid? ParentChainId => throw new NotImplementedException();

        public string BrowserUrl => throw new NotImplementedException();

        public long? AuditEventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AuditEventName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ServiceRequestMessagesDatabaseName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort MaxDeadlockRetries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Transaction Transaction => throw new NotImplementedException();

        public TransactionIsolationLevel? IsolationLevel => throw new NotImplementedException();

        public bool IsDisposed => throw new NotImplementedException();

        public IHttpContextAccess HttpContextAccess { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void BeginTransaction(TransactionIsolationLevel isolationLevel)
        {
        }

        public void BeginTransaction(TransactionIsolationLevel isolationLevel, TimeSpan timeout)
        {
        }

        public void ClearTransactionAndDataSources()
        {
        }

        public void Commit()
        {
        }

        public IContext CreateNew()
        {
            return this;
        }

        public void Dispose()
        {
        }

        public IDataSource EnsureDataSource(string databaseName)
        {
            throw new NotImplementedException();
        }

        public Task<IDataSource> EnsureDataSourceAsync(string databaseName)
        {
            throw new NotImplementedException();
        }

        public T EnsureRepository<T>() where T : IRepository
        {
            throw new NotImplementedException();
        }
    }
}
