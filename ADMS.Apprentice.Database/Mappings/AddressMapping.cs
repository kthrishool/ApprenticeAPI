using ADMS.Apprentice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADMS.Apprentice.Database.Mappings
{
    internal class AddressMapping : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> entity)
        {
            entity.ToTable("ApprenticeAddress", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("ApprenticeAddressId")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(e => e.ApprenticeId)
                .HasColumnName("ApprenticeId")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.AddressTypeCode)
                .HasColumnName("AddressTypeCode")
                .HasMaxLength(10)
                .IsRequired();
            entity.Property(e => e.StreetAddress1)
                .HasColumnName("StreetAddress1")
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(e => e.StreetAddress2)
                .HasColumnName("StreetAddress2")
                .HasMaxLength(100);
            entity.Property(e => e.StreetAddress3)
                .HasColumnName("StreetAddress3")
                .HasMaxLength(100);


            entity.Property(e => e.Locality)
                .HasColumnName("Locality")
                .HasMaxLength(50);
                //.IsRequired();
            entity.Property(e => e.StateCode)
                .HasColumnName("StateCode")
                .HasMaxLength(10)
                .IsRequired();
            entity.Property(e => e.Postcode)
                .HasColumnName("Postcode")
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(e => e.SingleLineAddress)
                .HasColumnName("SingleLineAddress")
                .HasMaxLength(375)
                .IsRequired();
            entity.Property(e => e.GeocodeType)
                .HasColumnName("GeocodeType")
                .HasMaxLength(4);
            entity.Property(e => e.Latitude)
                .HasColumnName("Latitude");

            entity.Property(e => e.Longitude)
                .HasColumnName("Longitude");

            entity.Property(e => e.Confidence)
                .HasColumnName("Confidence");


            entity.Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired()
                .IsRowVersion();
            entity.Property(e => e.AuditEventId)
                .HasColumnName("_AuditEventId");

            entity.HasOne(e => e.Profile)
                .WithMany(c => c.Addresses)
                .HasForeignKey(e => e.ApprenticeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}