using ADMS.Apprentice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ADMS.Apprentice.Database.Mappings
{
    internal class TfnStatusHistoryMapping : IEntityTypeConfiguration<TfnStatusHistory>
    {
        public void Configure(EntityTypeBuilder<TfnStatusHistory> entity)
        {
            entity.ToTable("TfnStatusHistory", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("TfnStatusHistoryId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.TfnDetailId)
                .HasColumnName("TfnDetailId")
                .IsRequired();
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