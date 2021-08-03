using ADMS.Apprentices.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADMS.Apprentices.Database.Mappings
{
    internal class ApprenticeUSIMapping : IEntityTypeConfiguration<ApprenticeUSI>
    {
        public void Configure(EntityTypeBuilder<ApprenticeUSI> entity)
        {
            entity.ToTable("ApprenticeUSI", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("ApprenticeUSIId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ApprenticeId)
                .HasColumnName("ApprenticeId")
                .IsRequired();
            entity.Property(e => e.USI)
                .HasColumnName("USI")
                .IsUnicode()
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.ActiveFlag)
                .HasColumnName("ActiveFlag")
                .IsRequired();
            entity.Property(e => e.USIChangeReason)
                .HasColumnName("USIChangeReason")
                .HasMaxLength(300);
            entity.Property(e => e.USIVerifyFlag)
                .HasColumnName("USIVerifyFlag");
            entity.Property(e => e.FirstNameMatchedFlag)
                .HasColumnName("FirstNameMatchedFlag");
            entity.Property(e => e.SurnameMatchedFlag)
                .HasColumnName("SurnameMatchedFlag");
            entity.Property(e => e.DateOfBirthMatchedFlag)
                .HasColumnName("DateOfBirthMatchedFlag");
            entity.Property(e => e.USIStatus)
                .HasColumnName("USIStatus")
                .HasMaxLength(15);

            entity.HasOne(x => x.Profile)
                .WithMany(c => c.USIs)
                .HasForeignKey(e => e.ApprenticeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Version)
                .HasColumnName("Version")
                .IsRequired()
                .IsRowVersion();

            entity.Property(e => e.AuditEventId)
                .HasColumnName("_AuditEventId");
        }
    }
}