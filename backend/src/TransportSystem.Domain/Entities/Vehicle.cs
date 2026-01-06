using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Domain.Entities;

/// <summary>
/// Represents a vehicle (bus) in the transport system
/// Vehicles can be soft-deleted to maintain historical data
/// </summary>
public class Vehicle : SoftDeletableEntity
{
    /// <summary>
    /// Registration number/license plate of the vehicle (unique)
    /// </summary>
    public string RegistrationNumber { get; private set; }

    /// <summary>
    /// Model/type of the vehicle (e.g., "Mercedes-Benz Citaro", "MAN Lion's City")
    /// </summary>
    public string Model { get; private set; }

    /// <summary>
    /// Seating capacity of the vehicle
    /// </summary>
    public int Capacity { get; private set; }

    /// <summary>
    /// Year the vehicle was manufactured
    /// </summary>
    public int ManufactureYear { get; private set; }

    /// <summary>
    /// Current status of the vehicle
    /// </summary>
    public VehicleStatus Status { get; private set; }

    /// <summary>
    /// Optional notes about the vehicle
    /// </summary>
    public string? Notes { get; private set; }

    // EF Core constructor
    private Vehicle() : base()
    {
        RegistrationNumber = string.Empty;
        Model = string.Empty;
    }

    private Vehicle(
        string registrationNumber,
        string model,
        int capacity,
        int manufactureYear,
        VehicleStatus status = VehicleStatus.Active,
        string? notes = null) : base()
    {
        RegistrationNumber = registrationNumber;
        Model = model;
        Capacity = capacity;
        ManufactureYear = manufactureYear;
        Status = status;
        Notes = notes;
    }

    /// <summary>
    /// Creates a new vehicle with validation
    /// </summary>
    public static Vehicle Create(
        string registrationNumber,
        string model,
        int capacity,
        int manufactureYear,
        VehicleStatus status = VehicleStatus.Active,
        string? notes = null)
    {
        // Validate registration number
        if (string.IsNullOrWhiteSpace(registrationNumber))
            throw new DomainException("Vehicle registration number cannot be empty");

        if (registrationNumber.Length > 20)
            throw new DomainException($"Registration number cannot exceed 20 characters. Provided: {registrationNumber.Length}");

        // Validate model
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Vehicle model cannot be empty");

        if (model.Length > 50)
            throw new DomainException($"Vehicle model cannot exceed 50 characters. Provided: {model.Length}");

        // Validate capacity
        if (capacity <= 0)
            throw new DomainException($"Vehicle capacity must be greater than 0. Provided: {capacity}");

        if (capacity > 200)
            throw new DomainException($"Vehicle capacity cannot exceed 200. Provided: {capacity}");

        // Validate manufacture year
        var currentYear = DateTime.UtcNow.Year;
        if (manufactureYear < 1900 || manufactureYear > currentYear + 1)
            throw new DomainException($"Invalid manufacture year. Must be between 1900 and {currentYear + 1}. Provided: {manufactureYear}");

        // Validate notes
        if (notes != null && notes.Length > 500)
            throw new DomainException($"Vehicle notes cannot exceed 500 characters. Provided: {notes.Length}");

        return new Vehicle(
            registrationNumber.Trim().ToUpperInvariant(),
            model.Trim(),
            capacity,
            manufactureYear,
            status,
            notes?.Trim());
    }

    /// <summary>
    /// Updates vehicle information
    /// </summary>
    public void Update(string model, int capacity, int manufactureYear, string? notes = null)
    {
        // Validate model
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Vehicle model cannot be empty");

        if (model.Length > 50)
            throw new DomainException($"Vehicle model cannot exceed 50 characters. Provided: {model.Length}");

        // Validate capacity
        if (capacity <= 0)
            throw new DomainException($"Vehicle capacity must be greater than 0. Provided: {capacity}");

        if (capacity > 200)
            throw new DomainException($"Vehicle capacity cannot exceed 200. Provided: {capacity}");

        // Validate manufacture year
        var currentYear = DateTime.UtcNow.Year;
        if (manufactureYear < 1900 || manufactureYear > currentYear + 1)
            throw new DomainException($"Invalid manufacture year. Must be between 1900 and {currentYear + 1}. Provided: {manufactureYear}");

        // Validate notes
        if (notes != null && notes.Length > 500)
            throw new DomainException($"Vehicle notes cannot exceed 500 characters. Provided: {notes.Length}");

        // Update properties
        Model = model.Trim();
        Capacity = capacity;
        ManufactureYear = manufactureYear;
        Notes = notes?.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Changes the vehicle status
    /// </summary>
    public void ChangeStatus(VehicleStatus newStatus)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if the vehicle is available for assignment
    /// </summary>
    public bool IsAvailable()
    {
        return Status == VehicleStatus.Active && !IsDeleted;
    }

    public override string ToString()
    {
        return $"{Model} ({RegistrationNumber})";
    }
}

/// <summary>
/// Enum representing vehicle status
/// </summary>
public enum VehicleStatus
{
    Active = 0,
    Maintenance = 1,
    OutOfService = 2
}
