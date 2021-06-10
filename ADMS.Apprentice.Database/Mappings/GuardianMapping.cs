using ADMS.Apprentice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADMS.Apprentice.Database.Mappings
{
    internal class GuardianMapping : IEntityTypeConfiguration<Guardian>
    {
        public void Configure(EntityTypeBuilder<Guardian> entity)
        {
            entity.ToTable("Guardian", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("GuardianId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ApprenticeId)
                .HasColumnName("ApprenticeId")
                .IsRequired();
            entity.Property(e => e.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.Surname)
                .HasColumnName("Surname")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.EmailAddress)
                .HasColumnName("EmailAddress")
                .HasMaxLength(320);
            entity.Property(e => e.HomePhoneNumber)
                .HasColumnName("HomePhoneNumber")
                .HasMaxLength(15);
            entity.Property(e => e.Mobile)
                .HasColumnName("Mobile")
                .HasMaxLength(15);
            entity.Property(e => e.WorkPhoneNumber)
                .HasColumnName("WorkPhoneNumber")
                .HasMaxLength(15);
            entity.Property(x => x.StreetAddress1)
                .HasColumnName("StreetAddress1")                
                .HasMaxLength(100);

            entity.Property(x => x.StreetAddress2)
                .HasColumnName("StreetAddress2")
                .HasMaxLength(100);

            entity.Property(x => x.StreetAddress3)
                .HasColumnName("StreetAddress3")
                .HasMaxLength(100);
            entity.Property(x => x.SingleLineAddress)
                .HasColumnName("SingleLineAddress")
                .HasMaxLength(375);

            entity.Property(x => x.Locality)
                .HasColumnName("Locality")                
                .HasMaxLength(50);

            entity.Property(x => x.StateCode)
                .HasColumnName("StateCode")                
                .HasMaxLength(10);

            entity.Property(x => x.Postcode)
                .HasColumnName("Postcode")                
                .HasMaxLength(10);

            entity.Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired()
                .IsRowVersion();
            entity.Property(e => e.AuditEventId)
                .HasColumnName("_AuditEventId");

            entity.HasOne(e => e.Profile)
                .WithOne(c => c.Guardian)
                .HasForeignKey<Guardian>(e => e.ApprenticeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}