
using System.Linq;
using System.Threading.Tasks;
using Adms.Shared.Database;
using ADMS.Apprentice.Core;
using ADMS.Apprentice.Core.Services;
using ADMS.Apprentice.Core.TYIMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ADMS.Apprentice.Database.Mappings.TYIMS;

namespace ADMS.Apprentice.Database
{
    public class TYIMSRepository : RepositoryBase, ITYIMSRepository
    {
        private readonly IOptions<OurDatabaseSettings> ourDatabaseSettings;

        public TYIMSRepository(IOptions<OurDatabaseSettings> ourDatabaseSettings, IContextRetriever contextRetriever, IAuditEventHelper auditEventHelper)
                : base(contextRetriever, auditEventHelper)
        {
            this.ourDatabaseSettings = ourDatabaseSettings;
        }

        protected override string DatabaseConnectionString => ourDatabaseSettings.Value.TYIMSConnectionString;
        public async Task<Registration> GetRegistrationAsync(int registrationId)
        {
            var registrations = await Set<Registration>()
                .FromSqlInterpolated($"[adms].ApprenticeApplication_RegistrationQualification_GetByRegistrationId @RegistrationId={registrationId}")
                .ToArrayAsync()
                ;
            return registrations.SingleOrDefault();
        }
        
        protected override void ApplyMappings(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RegistrationMapping());
        }
    }
}