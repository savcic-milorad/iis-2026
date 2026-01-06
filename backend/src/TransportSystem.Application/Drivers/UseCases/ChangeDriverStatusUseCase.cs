using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Drivers.DTOs;
using TransportSystem.Application.Persistence;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Drivers.UseCases;

/// <summary>
/// Use case for changing driver status (Active, OnLeave, Suspended)
/// </summary>
public class ChangeDriverStatusUseCase
{
    private readonly IApplicationDbContext _context;

    public ChangeDriverStatusUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DriverResponseDto> ExecuteAsync(Guid id, DriverStatus newStatus)
    {
        // Find driver
        var driver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", id);
        }

        // Change status using domain method
        driver.ChangeStatus(newStatus);

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
