using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Domain.Entities;

namespace TransportSystem.Infrastructure.Persistence.Configurations;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("Stations");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        // Configure GPS Coordinates as owned type
        builder.OwnsOne(s => s.Coordinates, coordinates =>
        {
            coordinates.Property(c => c.Latitude)
                .IsRequired()
                .HasColumnName("Latitude")
                .HasPrecision(10, 8); // Precision for GPS coordinates

            coordinates.Property(c => c.Longitude)
                .IsRequired()
                .HasColumnName("Longitude")
                .HasPrecision(11, 8); // Precision for GPS coordinates
        });

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.DeletedAt);

        // Indexes
        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.IsDeleted);
        builder.HasIndex(s => s.CreatedAt);
    }
}
