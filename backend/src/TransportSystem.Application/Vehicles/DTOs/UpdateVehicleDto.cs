using System.ComponentModel.DataAnnotations;

namespace TransportSystem.Application.Vehicles.DTOs;

/// <summary>
/// DTO for updating an existing vehicle
/// </summary>
public class UpdateVehicleDto
{
    /// <summary>
    /// Model/type of the vehicle (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "Vehicle model is required")]
    [StringLength(50, ErrorMessage = "Vehicle model cannot exceed 50 characters")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Seating capacity (required, 1-200)
    /// </summary>
    [Required(ErrorMessage = "Vehicle capacity is required")]
    [Range(1, 200, ErrorMessage = "Vehicle capacity must be between 1 and 200")]
    public int Capacity { get; set; }

    /// <summary>
    /// Year the vehicle was manufactured (required)
    /// </summary>
    [Required(ErrorMessage = "Manufacture year is required")]
    [Range(1900, 2027, ErrorMessage = "Invalid manufacture year")]
    public int ManufactureYear { get; set; }

    /// <summary>
    /// Optional notes about the vehicle (max 500 characters)
    /// </summary>
    [StringLength(500, ErrorMessage = "Vehicle notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
