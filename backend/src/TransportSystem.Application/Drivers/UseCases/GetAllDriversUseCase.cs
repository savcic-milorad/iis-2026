using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Drivers.DTOs;
using TransportSystem.Application.Persistence;
using TransportSystem.Domain.Entities;

namespace TransportSystem.Application.Drivers.UseCases;

/// <summary>
/// Use case for retrieving all drivers with optional filtering and search
/// </summary>
public class GetAllDriversUseCase
{
    private readonly IApplicationDbContext _context;

    public GetAllDriversUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all drivers with optional search term and status filter
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter by name, license number, or phone number</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="includeDeleted">Whether to include soft-deleted drivers (default: false)</param>
    public async Task<List<DriverResponseDto>> ExecuteAsync(
        string? searchTerm = null,
        DriverStatus? status = null,
        bool includeDeleted = false)
    {
        // Start with base query
        IQueryable<Driver> query = _context.Drivers;

        // Include deleted drivers if requested
        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();
            query = query.Where(d =>
                d.FullName.ToLower().Contains(searchTermLower) ||
                d.LicenseNumber.ToLower().Contains(searchTermLower) ||
                d.PhoneNumber.Contains(searchTermLower) ||
                (d.Notes != null && d.Notes.ToLower().Contains(searchTermLower))
            );
        }

        // Apply status filter if provided
        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }

        // Order by full name
        query = query.OrderBy(d => d.FullName);

        // Execute query and map to DTOs
        var drivers = await query.ToListAsync();

        return drivers.Select(MapToResponseDto).ToList();
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
