using ADMS.Apprentices.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADMS.Apprentices.Database.Mappings
{
    internal class PhoneMapping : IEntityTypeConfiguration<Phone>
    {
        public void Configure(EntityTypeBuilder<Phone> entity)
        {
            entity.ToTable("ApprenticePhone", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("ApprenticePhoneId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ApprenticeId)
                .HasColumnName("ApprenticeId")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.PhoneTypeCode)
                .HasColumnName("PhoneTypeCode")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.PhoneNumber)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(15)
                .IsRequired();
            entity.Property(e => e.PreferredPhoneFlag)
                .HasColumnName("PreferredPhoneFlag");                

            entity.Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired()
                .IsRowVersion();
            entity.Property(e => e.AuditEventId)
                .HasColumnName("_AuditEventId");

            entity.HasOne(e => e.Profile)
                .WithMany(c => c.Phones)
                .HasForeignKey(e => e.ApprenticeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}