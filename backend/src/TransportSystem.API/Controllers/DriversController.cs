using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Application.Drivers.DTOs;
using TransportSystem.Application.Drivers.UseCases;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.API.Controllers;

/// <summary>
/// Controller for managing drivers
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly CreateDriverUseCase _createDriver;
    private readonly UpdateDriverUseCase _updateDriver;
    private readonly DeleteDriverUseCase _deleteDriver;
    private readonly GetDriverUseCase _getDriver;
    private readonly GetAllDriversUseCase _getAllDrivers;
    private readonly ChangeDriverStatusUseCase _changeDriverStatus;
    private readonly ILogger<DriversController> _logger;

    public DriversController(
        CreateDriverUseCase createDriver,
        UpdateDriverUseCase updateDriver,
        DeleteDriverUseCase deleteDriver,
        GetDriverUseCase getDriver,
        GetAllDriversUseCase getAllDrivers,
        ChangeDriverStatusUseCase changeDriverStatus,
        ILogger<DriversController> logger)
    {
        _createDriver = createDriver;
        _updateDriver = updateDriver;
        _deleteDriver = deleteDriver;
        _getDriver = getDriver;
        _getAllDrivers = getAllDrivers;
        _changeDriverStatus = changeDriverStatus;
        _logger = logger;
    }

    /// <summary>
    /// Get all drivers with optional search and filters
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter by name, license number, or phone</param>
    /// <param name="status">Optional status filter (Active, OnLeave, Suspended)</param>
    /// <param name="includeDeleted">Whether to include soft-deleted drivers (default: false)</param>
    /// <returns>List of drivers</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(List<DriverResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<DriverResponseDto>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] DriverStatus? status = null,
        [FromQuery] bool includeDeleted = false)
    {
        try
        {
            var drivers = await _getAllDrivers.ExecuteAsync(searchTerm, status, includeDeleted);
            return Ok(drivers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving drivers");
            return StatusCode(500, new { message = "An error occurred while retrieving drivers" });
        }
    }

    /// <summary>
    /// Get a driver by ID
    /// </summary>
    /// <param name="id">Driver ID</param>
    /// <returns>Driver details</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(DriverResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverResponseDto>> GetById(Guid id)
    {
        try
        {
            var driver = await _getDriver.ExecuteAsync(id);
            return Ok(driver);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Driver not found: {DriverId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving driver: {DriverId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the driver" });
        }
    }

    /// <summary>
    /// Create a new driver (Admin and Planner only)
    /// </summary>
    /// <param name="dto">Driver creation data</param>
    /// <returns>Created driver</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(DriverResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DriverResponseDto>> Create([FromBody] CreateDriverDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var driver = await _createDriver.ExecuteAsync(dto);

            _logger.LogInformation(
                "Driver created: {DriverId} - {DriverName} (License: {LicenseNumber}) by user {UserId}",
                driver.Id,
                driver.FullName,
                driver.LicenseNumber,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return CreatedAtAction(
                nameof(GetById),
                new { id = driver.Id },
                driver);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while creating driver");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating driver");
            return StatusCode(500, new { message = "An error occurred while creating the driver" });
        }
    }

    /// <summary>
    /// Update an existing driver (Admin and Planner only)
    /// </summary>
    /// <param name="id">Driver ID</param>
    /// <param name="dto">Driver update data</param>
    /// <returns>Updated driver</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(DriverResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverResponseDto>> Update(Guid id, [FromBody] UpdateDriverDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var driver = await _updateDriver.ExecuteAsync(id, dto);

            _logger.LogInformation(
                "Driver updated: {DriverId} - {DriverName} by user {UserId}",
                driver.Id,
                driver.FullName,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return Ok(driver);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Driver not found for update: {DriverId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while updating driver: {DriverId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating driver: {DriverId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the driver" });
        }
    }

    /// <summary>
    /// Change driver status (Admin and Planner only)
    /// </summary>
    /// <param name="id">Driver ID</param>
    /// <param name="newStatus">New status (Active, OnLeave, Suspended)</param>
    /// <returns>Updated driver</returns>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(DriverResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverResponseDto>> ChangeStatus(Guid id, [FromBody] DriverStatus newStatus)
    {
        try
        {
            var driver = await _changeDriverStatus.ExecuteAsync(id, newStatus);

            _logger.LogInformation(
                "Driver status changed: {DriverId} - {DriverName} to {NewStatus} by user {UserId}",
                driver.Id,
                driver.FullName,
                newStatus,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return Ok(driver);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Driver not found for status change: {DriverId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while changing driver status: {DriverId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing driver status: {DriverId}", id);
            return StatusCode(500, new { message = "An error occurred while changing the driver status" });
        }
    }

    /// <summary>
    /// Soft delete a driver (Admin and Planner only)
    /// </summary>
    /// <param name="id">Driver ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _deleteDriver.ExecuteAsync(id);

            _logger.LogInformation(
                "Driver deleted: {DriverId} by user {UserId}",
                id,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Driver not found for deletion: {DriverId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while deleting driver: {DriverId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting driver: {DriverId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the driver" });
        }
    }
}
