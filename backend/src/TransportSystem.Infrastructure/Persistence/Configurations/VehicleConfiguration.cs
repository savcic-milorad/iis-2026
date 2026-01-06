using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Domain.Entities;

namespace TransportSystem.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .ValueGeneratedNever();

        builder.Property(v => v.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Capacity)
            .IsRequired();

        builder.Property(v => v.ManufactureYear)
            .IsRequired();

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>() // Store enum as string
            .HasMaxLength(20);

        builder.Property(v => v.Notes)
            .HasMaxLength(500);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt);

        builder.Property(v => v.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(v => v.DeletedAt);

        // Indexes
        builder.HasIndex(v => v.RegistrationNumber)
            .IsUnique();

        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.IsDeleted);
        builder.HasIndex(v => v.CreatedAt);
    }
}
