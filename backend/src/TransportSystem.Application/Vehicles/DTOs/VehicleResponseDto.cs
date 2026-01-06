using TransportSystem.Domain.Entities;

namespace TransportSystem.Application.Vehicles.DTOs;

/// <summary>
/// DTO for vehicle response data
/// </summary>
public class VehicleResponseDto
{
    /// <summary>
    /// Unique identifier of the vehicle
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Registration number/license plate
    /// </summary>
    public string RegistrationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Model/type of the vehicle
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Seating capacity
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Year the vehicle was manufactured
    /// </summary>
    public int ManufactureYear { get; set; }

    /// <summary>
    /// Current status of the vehicle
    /// </summary>
    public VehicleStatus Status { get; set; }

    /// <summary>
    /// Whether the vehicle is available for assignment
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Optional notes about the vehicle
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the vehicle record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the vehicle record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Whether the vehicle is deleted (for admin purposes)
    /// </summary>
    public bool IsDeleted { get; set; }
}
