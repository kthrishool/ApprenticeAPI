using ADMS.Apprentice.Core;
using ADMS.Apprentice.Database.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentice.Database
{
    public abstract class RepositoryBase : DbContext
    {
        private readonly OurDatabaseSettings ourDatabaseSettings;

        protected RepositoryBase(IOptions<OurDatabaseSettings> ourDatabaseSettings)
        {
            this.ourDatabaseSettings = ourDatabaseSettings.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer(ourDatabaseSettings.DatabaseConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClaimSubmissionMapping());
        }
    }
}