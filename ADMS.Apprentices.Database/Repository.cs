using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADMS.Apprentices.Core;
using ADMS.Apprentices.Core.Messages;
using ADMS.Apprentices.Core.Models;
using ADMS.Apprentices.Core.Services;
using ADMS.Apprentices.Database.Mappings;
using Adms.Shared.Database;
using Adms.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Au.Gov.Infrastructure.EntityFramework.Services;

namespace ADMS.Apprentices.Database
{
    public class Repository : RepositoryBase, IApprenticeRepository
    {

        public Repository(
            IContextRetriever contextRetriever,
            IDbContextConfigurator dbContextConfigurator,
            IAuditInformationUpdater auditInformationUpdater,
            IAuditEventService auditEventService
        ) : base(contextRetriever, dbContextConfigurator, auditInformationUpdater, auditEventService)
        {
        }

        public async Task<ICollection<ProfileSearchResultModel>> GetProfilesAsync(ProfileSearchMessage searchMessage)
        {
            FormattableString query = $@"ApprenticeAdvancedSearch @Names = {searchMessage.Name}, @ApprenticeID = {searchMessage.ApprenticeID}, 
                @BirthDate = {searchMessage.BirthDate}, @EmailAddress = {searchMessage.EmailAddress}, 
                @USI = {searchMessage.USI}, @PhoneNumber = {searchMessage.Phonenumber}, @AddressString = {searchMessage.Address}";

            return await Set<ProfileSearchResultModel>()
                .FromSqlInterpolated(query).ToListAsync();
        }

        public async Task<ApprenticeIdentitySearchResultModel[]> GetMatchesByIdentityAsync(ApprenticeIdentitySearchCriteriaMessage message)
        {
            FormattableString query = $@"ApprenticeBasicSearch 
                @FirstName={message.FirstName.ToNullIfEmpty()}, 
                @Surname={message.Surname.ToNullIfEmpty()}, 
                @BirthDate={message.BirthDate}, 
                @USI={message.USI.ToNullIfEmpty()}, 
                @EmailAddress={message.EmailAddress.ToNullIfEmpty()}, 
                @PhoneNumber={message.PhoneNumber.ToNullIfEmpty()}";
            return await Set<ApprenticeIdentitySearchResultModel>().FromSqlInterpolated(query).ToArrayAsync();
        }

        protected override void ApplyMappings(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProfileMapping());
            modelBuilder.ApplyConfiguration(new ApprenticeTFNMapping());
            modelBuilder.ApplyConfiguration(new PhoneMapping());
            modelBuilder.ApplyConfiguration(new AddressMapping());
            modelBuilder.ApplyConfiguration(new PriorQualificationMapping());
            modelBuilder.ApplyConfiguration(new ApprenticeUSIMapping());
            modelBuilder.ApplyConfiguration(new GuardianMapping());
            modelBuilder.ApplyConfiguration(new PriorApprenticeshipQualificationMapping());
            modelBuilder.Entity<ProfileSearchResultModel>().HasKey("ApprenticeId");
            modelBuilder.Entity<ApprenticeIdentitySearchResultModel>().HasKey("ApprenticeId");
        }
    }
}