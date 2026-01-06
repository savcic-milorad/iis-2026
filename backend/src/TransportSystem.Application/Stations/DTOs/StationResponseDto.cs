namespace TransportSystem.Application.Stations.DTOs;

/// <summary>
/// DTO for station response data
/// </summary>
public class StationResponseDto
{
    /// <summary>
    /// Unique identifier of the station
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the station
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// GPS latitude coordinate
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// GPS longitude coordinate
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Address of the station
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the station
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// When the station was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the station was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Whether the station is deleted (for admin purposes)
    /// </summary>
    public bool IsDeleted { get; set; }
}
