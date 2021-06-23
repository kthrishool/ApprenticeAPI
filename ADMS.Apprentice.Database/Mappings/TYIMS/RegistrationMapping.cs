
using ADMS.Apprentice.Core.TYIMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADMS.Apprentice.Database.Mappings.TYIMS
{
    public class RegistrationMapping : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> entity)
        {
            entity.Property(e => e.RegistrationId);
            entity.Property(e => e.StartDate);
            entity.Property(e => e.EndDate)
                .HasColumnName("CurrentActualEndDate");
            entity.Property(e => e.QualificationCode);
            entity.Property(e => e.TrainingContractId)
                .HasColumnName("ApplicationID");
            entity.Property(e => e.CurrentEndReasonCode);
        }
    }
}