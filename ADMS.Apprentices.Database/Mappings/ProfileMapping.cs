using ADMS.Apprentices.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADMS.Apprentices.Database.Mappings
{
    internal class ProfileMapping : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> entity)
        {
            entity.ToTable("Apprentice", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("ApprenticeId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Surname)
                .HasColumnName("Surname")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.OtherNames)
                .HasColumnName("OtherNames")
                .HasMaxLength(50);
            entity.Property(e => e.PreferredName)
                .HasColumnName("PreferredName")
                .HasMaxLength(50);
            entity.Property(e => e.BirthDate)
                .HasColumnName("BirthDate")
                .HasColumnType("date")
                .IsRequired();
            entity.Property(e => e.GenderCode)
                .HasColumnName("GenderCode");
            entity.Property(e => e.EmailAddress)
                .HasColumnName("EmailAddress")
                .HasMaxLength(320);
            entity.Property(e => e.SelfAssessedDisabilityCode)
                .HasColumnName("SelfAssessedDisabilityCode");
            entity.Property(e => e.IndigenousStatusCode)
                .HasColumnName("IndigenousStatusCode");
            entity.Property(e => e.CitizenshipCode)
                .HasColumnName("CitizenshipCode");
            entity.Property(e => e.HighestSchoolLevelCode)
                .HasColumnName("HighestCompletedSchoolLevelCode");           
            entity.Property(e => e.LeftSchoolDate)
                .HasColumnName("LeftSchoolDate")
                .HasColumnType("date");
            entity.Property(e => e.ProfileTypeCode)
                .HasColumnName("ProfileTypeCode");
            entity.Property(e => e.LanguageCode)
                .HasColumnName("LanguageCode");
            entity.Property(e => e.CountryOfBirthCode)
                .HasColumnName("CountryOfBirthCode");
            entity.Property(e => e.PreferredContactType)
                .HasColumnName("PreferredContactType");
            entity.Property(e => e.VisaNumber)
                .HasColumnName("VisaNumber");
            entity.Property(e => e.ActiveFlag)
                .HasColumnName("ActiveFlag");
            entity.Property(e => e.DeceasedFlag)
                .HasColumnName("DeceasedFlag");
            entity.Property(e => e.NotPovidingUSIReasonCode)
                .HasColumnName("NotPovidingUSIReasonCode");
            entity.Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired()
                .IsRowVersion();
            entity.Property(e => e.AuditEventId)
                .HasColumnName("_AuditEventId");
        }
    }
}