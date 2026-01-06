using System.ComponentModel.DataAnnotations;
using TransportSystem.Domain.Entities;

namespace TransportSystem.Application.Drivers.DTOs;

/// <summary>
/// DTO for creating a new driver
/// </summary>
public class CreateDriverDto
{
    /// <summary>
    /// Full name of the driver (required, max 100 characters)
    /// </summary>
    [Required(ErrorMessage = "Driver full name is required")]
    [StringLength(100, ErrorMessage = "Driver full name cannot exceed 100 characters")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Driver's license number (required, max 50 characters, unique)
    /// </summary>
    [Required(ErrorMessage = "License number is required")]
    [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters")]
    public string LicenseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Driver's phone number (required, max 20 characters)
    /// </summary>
    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date when the driver's license was issued (required)
    /// </summary>
    [Required(ErrorMessage = "License issued date is required")]
    public DateTime LicenseIssuedDate { get; set; }

    /// <summary>
    /// Date when the driver's license expires (required)
    /// </summary>
    [Required(ErrorMessage = "License expiry date is required")]
    public DateTime LicenseExpiryDate { get; set; }

    /// <summary>
    /// Initial status of the driver (defaults to Active)
    /// </summary>
    public DriverStatus Status { get; set; } = DriverStatus.Active;

    /// <summary>
    /// Optional user ID to link the driver to a system user account
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Optional notes about the driver (max 500 characters)
    /// </summary>
    [StringLength(500, ErrorMessage = "Driver notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
