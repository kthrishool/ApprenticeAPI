using ADMS.Apprentice.Core;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentice.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Repository>
    {
        private class MockDatabaseSettings : IOptions<OurDatabaseSettings>
        {
            public OurDatabaseSettings Value => new() { DatabaseConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ADMSApprentice;Data Source=SKSQL01D.dev.construction.enet;" };
        }

        public Repository CreateDbContext(string[] args)
        {
            return new Repository(new MockDatabaseSettings(), null, null);
        }
    }
}