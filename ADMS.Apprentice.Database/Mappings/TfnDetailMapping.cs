using ADMS.Apprentice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ADMS.Apprentice.Database.Mappings
{
    internal class TfnDetailMapping : IEntityTypeConfiguration<TfnDetail>
    {
        public void Configure(EntityTypeBuilder<TfnDetail> entity)
        {
            entity.ToTable("TfnDetail", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("TfnDetailId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ApprenticeId)
                .HasColumnName("ApprenticeId")
                .IsRequired();
            entity.Property(e => e.TFN)
                .HasColumnName("TFN")
                .IsRequired()
                .HasMaxLength(FieldLength.Short);
            entity.Property(e => e.Status)
                .HasColumnName("Status")
                .HasConversion(new EnumToStringConverter<TfnStatus>())
                .IsRequired()
                .HasMaxLength(FieldLength.Short);
            entity.Property(e => e.Version)
                            .HasColumnName("Version")
                            .IsRequired()
                            .IsRowVersion();
        }
    }
}