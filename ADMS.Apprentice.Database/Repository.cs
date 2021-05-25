using ADMS.Apprentice.Core;
using ADMS.Apprentice.Database.Mappings;
using Adms.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ADMS.Apprentice.Core.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using ADMS.Apprentice.Core.Models;
using ADMS.Apprentice.Core.Messages;
using System.Linq;
using System.Collections;
using System;

namespace ADMS.Apprentice.Database
{
    public class Repository : RepositoryBase, IApprenticeRepository
    {
        private readonly IOptions<OurDatabaseSettings> ourDatabaseSettings;

        public Repository(IOptions<OurDatabaseSettings> ourDatabaseSettings, IContextRetriever contextRetriever, IAuditEventHelper auditEventHelper)
            : base(contextRetriever, auditEventHelper)
        {
            this.ourDatabaseSettings = ourDatabaseSettings;
        }

        protected override string DatabaseConnectionString => ourDatabaseSettings.Value.DatabaseConnectionString;

        public async Task<ICollection<ProfileSearchResultModel>> GetProfilesAsync(ProfileSearchMessage searchMessage)
        {
            FormattableString query = $"ApprenticeFuzzySearch @Surname = {searchMessage.Surname}, @FirstName = {searchMessage.FirstName}, @BirthDateStartRange = {searchMessage.BirthDate}, @BirthDateEndRange = {searchMessage.BirthDate}";
            return await Set<ProfileSearchResultModel>()
                .FromSqlInterpolated(query).ToListAsync();         
                   
        }

        protected override void ApplyMappings(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProfileMapping());
            modelBuilder.ApplyConfiguration(new ApprenticeTFNMapping());
            modelBuilder.ApplyConfiguration(new PhoneMapping());
            modelBuilder.ApplyConfiguration(new AddressMapping());
            modelBuilder.ApplyConfiguration(new QualificationMapping());
            modelBuilder.ApplyConfiguration(new ApprenticeUSIMapping());
            modelBuilder.ApplyConfiguration(new GuardianMapping());
            modelBuilder.Entity<ProfileSearchResultModel>().HasKey("ApprenticeId");
        }
    }
}