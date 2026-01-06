using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Domain.Entities;

/// <summary>
/// Represents a driver in the transport system
/// Drivers can be soft-deleted to maintain historical data
/// </summary>
public class Driver : SoftDeletableEntity
{
    /// <summary>
    /// Full name of the driver
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// Driver's license number (unique)
    /// </summary>
    public string LicenseNumber { get; private set; }

    /// <summary>
    /// Driver's phone number
    /// </summary>
    public string PhoneNumber { get; private set; }

    /// <summary>
    /// Date when the driver's license was issued
    /// </summary>
    public DateTime LicenseIssuedDate { get; private set; }

    /// <summary>
    /// Date when the driver's license expires
    /// </summary>
    public DateTime LicenseExpiryDate { get; private set; }

    /// <summary>
    /// Current status of the driver
    /// </summary>
    public DriverStatus Status { get; private set; }

    /// <summary>
    /// Optional reference to the ApplicationUser (if driver has system access)
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// Optional notes about the driver
    /// </summary>
    public string? Notes { get; private set; }

    // EF Core constructor
    private Driver() : base()
    {
        FullName = string.Empty;
        LicenseNumber = string.Empty;
        PhoneNumber = string.Empty;
    }

    private Driver(
        string fullName,
        string licenseNumber,
        string phoneNumber,
        DateTime licenseIssuedDate,
        DateTime licenseExpiryDate,
        DriverStatus status = DriverStatus.Active,
        string? userId = null,
        string? notes = null) : base()
    {
        FullName = fullName;
        LicenseNumber = licenseNumber;
        PhoneNumber = phoneNumber;
        LicenseIssuedDate = licenseIssuedDate;
        LicenseExpiryDate = licenseExpiryDate;
        Status = status;
        UserId = userId;
        Notes = notes;
    }

    /// <summary>
    /// Creates a new driver with validation
    /// </summary>
    public static Driver Create(
        string fullName,
        string licenseNumber,
        string phoneNumber,
        DateTime licenseIssuedDate,
        DateTime licenseExpiryDate,
        DriverStatus status = DriverStatus.Active,
        string? userId = null,
        string? notes = null)
    {
        // Validate full name
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Driver full name cannot be empty");

        if (fullName.Length > 100)
            throw new DomainException($"Driver full name cannot exceed 100 characters. Provided: {fullName.Length}");

        // Validate license number
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new DomainException("Driver license number cannot be empty");

        if (licenseNumber.Length > 50)
            throw new DomainException($"License number cannot exceed 50 characters. Provided: {licenseNumber.Length}");

        // Validate phone number
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Driver phone number cannot be empty");

        if (phoneNumber.Length > 20)
            throw new DomainException($"Phone number cannot exceed 20 characters. Provided: {phoneNumber.Length}");

        // Validate license dates
        if (licenseIssuedDate > DateTime.UtcNow)
            throw new DomainException("License issued date cannot be in the future");

        if (licenseExpiryDate <= licenseIssuedDate)
            throw new DomainException("License expiry date must be after the issued date");

        // Validate notes
        if (notes != null && notes.Length > 500)
            throw new DomainException($"Driver notes cannot exceed 500 characters. Provided: {notes.Length}");

        return new Driver(
            fullName.Trim(),
            licenseNumber.Trim().ToUpperInvariant(),
            phoneNumber.Trim(),
            licenseIssuedDate.Date,
            licenseExpiryDate.Date,
            status,
            userId,
            notes?.Trim());
    }

    /// <summary>
    /// Updates driver information
    /// </summary>
    public void Update(
        string fullName,
        string phoneNumber,
        DateTime licenseIssuedDate,
        DateTime licenseExpiryDate,
        string? notes = null)
    {
        // Validate full name
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Driver full name cannot be empty");

        if (fullName.Length > 100)
            throw new DomainException($"Driver full name cannot exceed 100 characters. Provided: {fullName.Length}");

        // Validate phone number
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Driver phone number cannot be empty");

        if (phoneNumber.Length > 20)
            throw new DomainException($"Phone number cannot exceed 20 characters. Provided: {phoneNumber.Length}");

        // Validate license dates
        if (licenseIssuedDate > DateTime.UtcNow)
            throw new DomainException("License issued date cannot be in the future");

        if (licenseExpiryDate <= licenseIssuedDate)
            throw new DomainException("License expiry date must be after the issued date");

        // Validate notes
        if (notes != null && notes.Length > 500)
            throw new DomainException($"Driver notes cannot exceed 500 characters. Provided: {notes.Length}");

        // Update properties
        FullName = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
        LicenseIssuedDate = licenseIssuedDate.Date;
        LicenseExpiryDate = licenseExpiryDate.Date;
        Notes = notes?.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Changes the driver status
    /// </summary>
    public void ChangeStatus(DriverStatus newStatus)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
        MarkAsUpdated();
    }

    /// <summary>
    /// Links the driver to a user account
    /// </summary>
    public void LinkToUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID cannot be empty");

        UserId = userId;
        MarkAsUpdated();
    }

    /// <summary>
    /// Unlinks the driver from a user account
    /// </summary>
    public void UnlinkFromUser()
    {
        UserId = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if the driver's license is currently valid
    /// </summary>
    public bool HasValidLicense()
    {
        return LicenseExpiryDate.Date >= DateTime.UtcNow.Date;
    }

    /// <summary>
    /// Checks if the driver is available for assignment
    /// </summary>
    public bool IsAvailable()
    {
        return Status == DriverStatus.Active && HasValidLicense() && !IsDeleted;
    }

    public override string ToString()
    {
        return $"{FullName} ({LicenseNumber})";
    }
}

/// <summary>
/// Enum representing driver status
/// </summary>
public enum DriverStatus
{
    Active = 0,
    OnLeave = 1,
    Suspended = 2
}
