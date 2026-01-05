using Microsoft.AspNetCore.Identity;

namespace TransportSystem.Infrastructure.Identity;

/// <summary>
/// Custom application user extending IdentityUser
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system (Admin, Planner, Driver, Passenger)
    /// </summary>
    public string Role { get; set; } = "Passenger";

    /// <summary>
    /// Date when the user account was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the user account was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates if the account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional reference to Driver entity (if user is a driver)
    /// </summary>
    public Guid? DriverId { get; set; }
}
