using TransportSystem.Domain.Exceptions;
using TransportSystem.Domain.ValueObjects;

namespace TransportSystem.Domain.Entities;

/// <summary>
/// Represents a bus/transport station in the system
/// Stations can be soft-deleted to maintain historical data
/// </summary>
public class Station : SoftDeletableEntity
{
    /// <summary>
    /// Name of the station (e.g., "Central Station", "Airport Terminal")
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// GPS coordinates of the station
    /// </summary>
    public GPSCoordinate Coordinates { get; private set; }

    /// <summary>
    /// Optional description or additional information about the station
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Address of the station
    /// </summary>
    public string Address { get; private set; }

    // EF Core constructor
    private Station() : base()
    {
        Name = string.Empty;
        Coordinates = null!;
        Address = string.Empty;
    }

    private Station(string name, GPSCoordinate coordinates, string address, string? description = null) : base()
    {
        Name = name;
        Coordinates = coordinates;
        Address = address;
        Description = description;
    }

    /// <summary>
    /// Creates a new station with validation
    /// </summary>
    public static Station Create(string name, double latitude, double longitude, string address, string? description = null)
    {
        // Validate name
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Station name cannot be empty");

        if (name.Length > 100)
            throw new DomainException($"Station name cannot exceed 100 characters. Provided: {name.Length}");

        // Validate address
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Station address cannot be empty");

        if (address.Length > 200)
            throw new DomainException($"Station address cannot exceed 200 characters. Provided: {address.Length}");

        // Validate description
        if (description != null && description.Length > 500)
            throw new DomainException($"Station description cannot exceed 500 characters. Provided: {description.Length}");

        // Create GPS coordinates (will validate latitude/longitude)
        var coordinates = GPSCoordinate.Create(latitude, longitude);

        return new Station(name.Trim(), coordinates, address.Trim(), description?.Trim());
    }

    /// <summary>
    /// Updates station information
    /// </summary>
    public void Update(string name, double latitude, double longitude, string address, string? description = null)
    {
        // Validate name
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Station name cannot be empty");

        if (name.Length > 100)
            throw new DomainException($"Station name cannot exceed 100 characters. Provided: {name.Length}");

        // Validate address
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Station address cannot be empty");

        if (address.Length > 200)
            throw new DomainException($"Station address cannot exceed 200 characters. Provided: {address.Length}");

        // Validate description
        if (description != null && description.Length > 500)
            throw new DomainException($"Station description cannot exceed 500 characters. Provided: {description.Length}");

        // Update properties
        Name = name.Trim();
        Coordinates = GPSCoordinate.Create(latitude, longitude);
        Address = address.Trim();
        Description = description?.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Calculates distance to another station in kilometers
    /// </summary>
    public double DistanceTo(Station other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return Coordinates.DistanceTo(other.Coordinates);
    }

    public override string ToString()
    {
        return $"{Name} ({Coordinates})";
    }
}
