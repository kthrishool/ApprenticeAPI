using Employment.Services.Infrastructure.Core.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDS.Services.Apprentice.UnitTests
{
    [ExcludeFromCodeCoverage]
    internal class MockDataSource : IDataSource
    {
        public bool IsDisposed { get; set; }

        public bool AuditEventRecordHasBeenCreated { get; set; }

        public IContext Context { get; set; }

        public IDbTransaction Transaction { get; set; }

        public IDbConnection Connection { get; set; }

        public void AddAuditEvent()
        {
            //throw new NotImplementedException();
        }

        public Task AddAuditEventAsync()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Rollback()
        {
            //throw new NotImplementedException();
        }
    }
}
