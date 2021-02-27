using System.Linq;
using ADMS.Apprentice.Core;
using ADMS.Apprentice.Database.Mappings;
using Adms.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace ADMS.Apprentice.Database
{
    public class Repository : RepositoryBase
    {
        private readonly IOptions<OurDatabaseSettings> ourDatabaseSettings;

        public Repository(IOptions<OurDatabaseSettings> ourDatabaseSettings, IContextRetriever contextRetriever, IAuditEventHelper auditEventHelper)
            : base(contextRetriever, auditEventHelper)
        {
            this.ourDatabaseSettings = ourDatabaseSettings;
        }

        protected override string DatabaseConnectionString => ourDatabaseSettings.Value.DatabaseConnectionString;

        protected override void ApplyMappings(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProfileMapping());
        }
    }
}