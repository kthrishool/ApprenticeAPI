using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMS.Apprentice.Core;
using ADMS.Apprentice.Core.Entities;
using ADMS.Apprentice.Core.Services;
using Adms.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ADMS.Apprentice.Database
{
    public class TyimsRepository : RepositoryBase, ITyimsRepository
    {
        private readonly IOptions<OurDatabaseSettings> ourDatabaseSettings;

        public TyimsRepository(
            IOptions<OurDatabaseSettings> ourDatabaseSettings,
            IContextRetriever contextRetriever,
            IAuditEventHelper auditEventHelper)
            : base(contextRetriever, auditEventHelper)
        {
            this.ourDatabaseSettings = ourDatabaseSettings;
        }

        protected override void ApplyMappings(ModelBuilder modelBuilder)
        {
            // modelBuilder.ApplyConfiguration(new CodeLocalityPostcodesStateMapping());
            modelBuilder.Entity<CodeLocalityPostcodesState>().HasNoKey();
        }

        protected override string DatabaseConnectionString => ourDatabaseSettings.Value.TyimsDatabaseConnectionString;

        public async Task<List<CodeLocalityPostcodesState>> GetPostCodeAsync(string postCodeID)
        {
            try
            {
                var results = await Set<CodeLocalityPostcodesState>()
                    .FromSqlInterpolated($"ApprenticeAddress_GetLocalityStateByPostcodes @Postcodes={postCodeID}")
                    .ToArrayAsync();
                return results.DefaultIfEmpty().ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}