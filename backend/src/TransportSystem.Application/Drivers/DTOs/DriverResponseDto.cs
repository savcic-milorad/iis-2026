using TransportSystem.Domain.Entities;

namespace TransportSystem.Application.Drivers.DTOs;

/// <summary>
/// DTO for driver response data
/// </summary>
public class DriverResponseDto
{
    /// <summary>
    /// Unique identifier of the driver
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Full name of the driver
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Driver's license number
    /// </summary>
    public string LicenseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Driver's phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date when the driver's license was issued
    /// </summary>
    public DateTime LicenseIssuedDate { get; set; }

    /// <summary>
    /// Date when the driver's license expires
    /// </summary>
    public DateTime LicenseExpiryDate { get; set; }

    /// <summary>
    /// Current status of the driver
    /// </summary>
    public DriverStatus Status { get; set; }

    /// <summary>
    /// Whether the driver's license is currently valid
    /// </summary>
    public bool HasValidLicense { get; set; }

    /// <summary>
    /// Whether the driver is available for assignment
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Optional user ID if linked to a system user account
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Optional notes about the driver
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the driver record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the driver record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Whether the driver is deleted (for admin purposes)
    /// </summary>
    public bool IsDeleted { get; set; }
}
