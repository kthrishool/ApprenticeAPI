using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentices.Core;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Core.TYIMS.Entities;
using ADMS.Apprentices.Database.Mappings.TYIMS;
using Adms.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentices.Database
{
    public class TYIMSRepository : DbContext, ITYIMSRepository
    {
        private readonly IOptions<OurDatabaseSettings> ourDatabaseSettings;

        public TYIMSRepository(IOptions<OurDatabaseSettings> ourDatabaseSettings)
        {
            this.ourDatabaseSettings = ourDatabaseSettings;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ourDatabaseSettings.Value.TYIMSConnectionString);
            }
        }

        public async Task<Registration> GetCompletedRegistrationsByApprenticeIdAsync(int apprenticeId)
        {
            var registrations = await Set<Registration>()
                    .FromSqlInterpolated($"[adms].ApprenticeApplication_RegistrationQualification_GetByRegistrationId @RegistrationId={apprenticeId}")
                    .ToArrayAsync()
                ;
            return registrations.SingleOrDefault();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RegistrationMapping());
        }
    }
}