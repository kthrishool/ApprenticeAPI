using ADMS.Apprentice.Core;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentice.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Repository>
    {
        public Repository CreateDbContext(string[] args)
        {
            var settings = new OurDatabaseSettings
            {
                DatabaseConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=adms;Data Source=(local)"
            };
            return new Repository(Options.Create(settings));
        }
    }
}