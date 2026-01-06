using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Domain.ValueObjects;

/// <summary>
/// Value object representing GPS coordinates (latitude and longitude)
/// </summary>
public class GPSCoordinate : IEquatable<GPSCoordinate>
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    private GPSCoordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Creates a new GPS coordinate with validation
    /// </summary>
    public static GPSCoordinate Create(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new DomainException($"Latitude must be between -90 and 90. Provided: {latitude}");

        if (longitude < -180 || longitude > 180)
            throw new DomainException($"Longitude must be between -180 and 180. Provided: {longitude}");

        return new GPSCoordinate(latitude, longitude);
    }

    /// <summary>
    /// Calculates the distance in kilometers to another GPS coordinate using the Haversine formula
    /// </summary>
    public double DistanceTo(GPSCoordinate other)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(Latitude)) * Math.Cos(DegreesToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public bool Equals(GPSCoordinate? other)
    {
        if (other is null)
            return false;

        return Math.Abs(Latitude - other.Latitude) < 0.000001 &&
               Math.Abs(Longitude - other.Longitude) < 0.000001;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as GPSCoordinate);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude);
    }

    public override string ToString()
    {
        return $"({Latitude:F6}, {Longitude:F6})";
    }

    public static bool operator ==(GPSCoordinate? a, GPSCoordinate? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(GPSCoordinate? a, GPSCoordinate? b)
    {
        return !(a == b);
    }
}
