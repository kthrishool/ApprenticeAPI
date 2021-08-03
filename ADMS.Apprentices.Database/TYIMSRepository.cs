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
    public class TYIMSRepository : RepositoryBase, ITYIMSRepository
    {
        private readonly IOptions<OurDatabaseSettings> ourDatabaseSettings;

        public TYIMSRepository(IOptions<OurDatabaseSettings> ourDatabaseSettings, IContextRetriever contextRetriever, IAuditEventHelper auditEventHelper)
            : base(contextRetriever, auditEventHelper)
        {
            this.ourDatabaseSettings = ourDatabaseSettings;
        }

        protected override string DatabaseConnectionString => ourDatabaseSettings.Value.TYIMSConnectionString;


        public async Task<Registration> GetCompletedRegistrationsByApprenticeIdAsync(int apprenticeId)
        {
            var registrations = await Set<Registration>()
                    .FromSqlInterpolated($"[adms].ApprenticeApplication_RegistrationQualification_GetByRegistrationId @RegistrationId={apprenticeId}")
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