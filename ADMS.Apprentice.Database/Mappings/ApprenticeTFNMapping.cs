using ADMS.Apprentice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ADMS.Apprentice.Database.Mappings
{
    internal class ApprenticeTFNMapping : IEntityTypeConfiguration<ApprenticeTFN>
    {
        public void Configure(EntityTypeBuilder<ApprenticeTFN> entity)
        {
            entity.ToTable("ApprenticeTFN", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("ApprenticeTFNId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ApprenticeId)
                .HasColumnName("ApprenticeId")
                .IsRequired();
            entity.Property(e => e.TaxFileNumber)
                .HasColumnName("TaxFileNumber")
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(20);
            entity.Property(e => e.StatusCode)
                .HasColumnName("StatusCode")
                .HasConversion(new EnumToStringConverter<TFNStatus>())
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.StatusDate)
                .HasColumnName("StatusDate")
                .IsRequired();

            entity.Property(e => e.StatusReasonCode)
                .HasColumnName("StatusReasonCode")
                .HasMaxLength(10);

            entity.Property(e => e.MessageQueueCorrelationId)
                .HasColumnName("MessageQueueCorrelationId")
                .IsRequired();

            entity.Property(e => e.Version)
                            .HasColumnName("Version")
                            .IsRequired()
                            .IsRowVersion();
        }
    }
}