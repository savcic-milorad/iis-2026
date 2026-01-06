using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Drivers.DTOs;
using TransportSystem.Application.Persistence;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Drivers.UseCases;

/// <summary>
/// Use case for updating an existing driver
/// </summary>
public class UpdateDriverUseCase
{
    private readonly IApplicationDbContext _context;

    public UpdateDriverUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DriverResponseDto> ExecuteAsync(Guid id, UpdateDriverDto dto)
    {
        // Find driver (ignoring soft delete filter)
        var driver = await _context.Drivers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", id);
        }

        // Check if driver is soft-deleted
        if (driver.IsDeleted)
        {
            throw new DomainException($"Cannot update a deleted driver. Please restore it first.");
        }

        // Update driver using domain entity method (validates internally)
        driver.Update(
            dto.FullName,
            dto.PhoneNumber,
            dto.LicenseIssuedDate,
            dto.LicenseExpiryDate,
            dto.Notes
        );

        // Save changes
        await _context.SaveChangesAsync();

        // Map to response DTO
        return MapToResponseDto(driver);
    }

    private static DriverResponseDto MapToResponseDto(Driver driver)
    {
        return new DriverResponseDto
        {
            Id = driver.Id,
            FullName = driver.FullName,
            LicenseNumber = driver.LicenseNumber,
            PhoneNumber = driver.PhoneNumber,
            LicenseIssuedDate = driver.LicenseIssuedDate,
            LicenseExpiryDate = driver.LicenseExpiryDate,
            Status = driver.Status,
            HasValidLicense = driver.HasValidLicense(),
            IsAvailable = driver.IsAvailable(),
            UserId = driver.UserId,
            Notes = driver.Notes,
            CreatedAt = driver.CreatedAt,
            UpdatedAt = driver.UpdatedAt,
            IsDeleted = driver.IsDeleted
        };
    }
}
