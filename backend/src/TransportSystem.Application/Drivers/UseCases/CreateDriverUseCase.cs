using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Drivers.DTOs;
using TransportSystem.Application.Persistence;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Drivers.UseCases;

/// <summary>
/// Use case for creating a new driver
/// </summary>
public class CreateDriverUseCase
{
    private readonly IApplicationDbContext _context;

    public CreateDriverUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DriverResponseDto> ExecuteAsync(CreateDriverDto dto)
    {
        // Check for duplicate license number
        var existingDriver = await _context.Drivers
            .Where(d => d.LicenseNumber == dto.LicenseNumber.ToUpperInvariant())
            .FirstOrDefaultAsync();

        if (existingDriver != null)
        {
            throw new DomainException($"A driver with license number '{dto.LicenseNumber}' already exists");
        }

        // Create driver using domain entity factory method (validates internally)
        var driver = Driver.Create(
            dto.FullName,
            dto.LicenseNumber,
            dto.PhoneNumber,
            dto.LicenseIssuedDate,
            dto.LicenseExpiryDate,
            dto.Status,
            dto.UserId,
            dto.Notes
        );

        // Add to database
        _context.Drivers.Add(driver);
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
