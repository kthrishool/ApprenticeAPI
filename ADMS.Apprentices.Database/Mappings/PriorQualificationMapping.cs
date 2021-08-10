using ADMS.Apprentices.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADMS.Apprentices.Database.Mappings
{
    internal class PriorQualificationMapping : IEntityTypeConfiguration<PriorQualification>
    {
        public void Configure(EntityTypeBuilder<PriorQualification> entity)
        {
            entity.ToTable("PriorQualification", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("PriorQualificationId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ApprenticeId)
                .HasColumnName("ApprenticeId")
                .IsRequired();
            entity.Property(e => e.QualificationCode)
                .HasColumnName("QualificationCode")
                .HasMaxLength(10)
                .IsRequired();
            entity.Property(e => e.QualificationDescription)
                .HasColumnName("QualificationDescription")
                .HasMaxLength(200);
            entity.Property(e => e.QualificationLevel)
                .HasColumnName("QualificationLevel")
                .HasMaxLength(10);
            entity.Property(e => e.QualificationANZSCOCode)
                .HasColumnName("QualificationANZSCOCode")
                .HasMaxLength(10);
            entity.Property(e => e.StartDate)
                .HasColumnName("StartDate");
            entity.Property(e => e.EndDate)
                .HasColumnName("EndDate");
            entity.Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired()
                .IsRowVersion();
            entity.Property(e => e.AuditEventId)
                .HasColumnName("_AuditEventId");

            entity.HasOne(e => e.Profile)
                .WithMany(c => c.PriorQualifications)
                .HasForeignKey(e => e.ApprenticeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}