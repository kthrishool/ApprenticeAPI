using ADMS.Apprentice.Core.ClaimSubmissions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADMS.Apprentice.Database.Mappings
{
    internal class ClaimSubmissionMapping : IEntityTypeConfiguration<ClaimSubmission>
    {
        public void Configure(EntityTypeBuilder<ClaimSubmission> entity)
        {
            entity.ToTable("ClaimSubmission", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.SubmissionStatus)
                .HasColumnName("SubmissionStatus")
                .IsRequired();
            entity.Property(e => e.Type)
                .HasColumnName("Type")
                .IsRequired();
            entity.Property(e => e.Category)
                .HasColumnName("Category")
                .IsRequired();
            entity.Property(e => e.ApprenticeId)
                .HasColumnName("ApprenticeId")
                .IsRequired();
            entity.Property(e => e.ApprenticeName)
                .HasColumnName("ApprenticeName")
                .IsRequired()
                .HasMaxLength(FieldLength.Normal);
            entity.Property(e => e.EmployerId)
                .HasColumnName("EmployerId")
                .IsRequired();
            entity.Property(e => e.EmployerName)
                .HasColumnName("EmployerName")
                .IsRequired()
                .HasMaxLength(FieldLength.Normal);
            entity.Property(e => e.NetworkProviderId)
                .HasColumnName("NetworkProviderId")
                .IsRequired();
            entity.Property(e => e.NetworkProviderName)
                .HasColumnName("NetworkProviderName")
                .IsRequired()
                .HasMaxLength(FieldLength.Normal);
            entity.Property(e => e.CreatedDate)
                .HasColumnName("CreatedDate")
                .IsRequired();
            entity.Property(e => e.LastModifiedDate)
                .HasColumnName("LastModifiedDate")
                .IsRequired();
        }
    }
}