using System.ComponentModel.DataAnnotations;

namespace TransportSystem.Application.Stations.DTOs;

/// <summary>
/// DTO for updating an existing station
/// </summary>
public class UpdateStationDto
{
    /// <summary>
    /// Name of the station (required, max 100 characters)
    /// </summary>
    [Required(ErrorMessage = "Station name is required")]
    [StringLength(100, ErrorMessage = "Station name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// GPS latitude coordinate (required, between -90 and 90)
    /// </summary>
    [Required(ErrorMessage = "Latitude is required")]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }

    /// <summary>
    /// GPS longitude coordinate (required, between -180 and 180)
    /// </summary>
    [Required(ErrorMessage = "Longitude is required")]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double Longitude { get; set; }

    /// <summary>
    /// Address of the station (required, max 200 characters)
    /// </summary>
    [Required(ErrorMessage = "Station address is required")]
    [StringLength(200, ErrorMessage = "Station address cannot exceed 200 characters")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the station (max 500 characters)
    /// </summary>
    [StringLength(500, ErrorMessage = "Station description cannot exceed 500 characters")]
    public string? Description { get; set; }
}
