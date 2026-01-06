using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Application.Vehicles.DTOs;
using TransportSystem.Application.Vehicles.UseCases;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.API.Controllers;

/// <summary>
/// Controller for managing vehicles
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly CreateVehicleUseCase _createVehicle;
    private readonly UpdateVehicleUseCase _updateVehicle;
    private readonly DeleteVehicleUseCase _deleteVehicle;
    private readonly GetVehicleUseCase _getVehicle;
    private readonly GetAllVehiclesUseCase _getAllVehicles;
    private readonly ChangeVehicleStatusUseCase _changeVehicleStatus;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(
        CreateVehicleUseCase createVehicle,
        UpdateVehicleUseCase updateVehicle,
        DeleteVehicleUseCase deleteVehicle,
        GetVehicleUseCase getVehicle,
        GetAllVehiclesUseCase getAllVehicles,
        ChangeVehicleStatusUseCase changeVehicleStatus,
        ILogger<VehiclesController> logger)
    {
        _createVehicle = createVehicle;
        _updateVehicle = updateVehicle;
        _deleteVehicle = deleteVehicle;
        _getVehicle = getVehicle;
        _getAllVehicles = getAllVehicles;
        _changeVehicleStatus = changeVehicleStatus;
        _logger = logger;
    }

    /// <summary>
    /// Get all vehicles with optional search and filters
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter by registration number, model, or notes</param>
    /// <param name="status">Optional status filter (Active, Maintenance, OutOfService)</param>
    /// <param name="includeDeleted">Whether to include soft-deleted vehicles (default: false)</param>
    /// <returns>List of vehicles</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(List<VehicleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<VehicleResponseDto>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] VehicleStatus? status = null,
        [FromQuery] bool includeDeleted = false)
    {
        try
        {
            var vehicles = await _getAllVehicles.ExecuteAsync(searchTerm, status, includeDeleted);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles");
            return StatusCode(500, new { message = "An error occurred while retrieving vehicles" });
        }
    }

    /// <summary>
    /// Get a vehicle by ID
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <returns>Vehicle details</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponseDto>> GetById(Guid id)
    {
        try
        {
            var vehicle = await _getVehicle.ExecuteAsync(id);
            return Ok(vehicle);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Vehicle not found: {VehicleId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle: {VehicleId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the vehicle" });
        }
    }

    /// <summary>
    /// Create a new vehicle (Admin and Planner only)
    /// </summary>
    /// <param name="dto">Vehicle creation data</param>
    /// <returns>Created vehicle</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<VehicleResponseDto>> Create([FromBody] CreateVehicleDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var vehicle = await _createVehicle.ExecuteAsync(dto);

            _logger.LogInformation(
                "Vehicle created: {VehicleId} - {RegistrationNumber} ({Model}) by user {UserId}",
                vehicle.Id,
                vehicle.RegistrationNumber,
                vehicle.Model,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return CreatedAtAction(
                nameof(GetById),
                new { id = vehicle.Id },
                vehicle);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while creating vehicle");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle");
            return StatusCode(500, new { message = "An error occurred while creating the vehicle" });
        }
    }

    /// <summary>
    /// Update an existing vehicle (Admin and Planner only)
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <param name="dto">Vehicle update data</param>
    /// <returns>Updated vehicle</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponseDto>> Update(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var vehicle = await _updateVehicle.ExecuteAsync(id, dto);

            _logger.LogInformation(
                "Vehicle updated: {VehicleId} - {RegistrationNumber} by user {UserId}",
                vehicle.Id,
                vehicle.RegistrationNumber,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return Ok(vehicle);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Vehicle not found for update: {VehicleId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while updating vehicle: {VehicleId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle: {VehicleId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the vehicle" });
        }
    }

    /// <summary>
    /// Change vehicle status (Admin and Planner only)
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <param name="newStatus">New status (Active, Maintenance, OutOfService)</param>
    /// <returns>Updated vehicle</returns>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponseDto>> ChangeStatus(Guid id, [FromBody] VehicleStatus newStatus)
    {
        try
        {
            var vehicle = await _changeVehicleStatus.ExecuteAsync(id, newStatus);

            _logger.LogInformation(
                "Vehicle status changed: {VehicleId} - {RegistrationNumber} to {NewStatus} by user {UserId}",
                vehicle.Id,
                vehicle.RegistrationNumber,
                newStatus,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return Ok(vehicle);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Vehicle not found for status change: {VehicleId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while changing vehicle status: {VehicleId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing vehicle status: {VehicleId}", id);
            return StatusCode(500, new { message = "An error occurred while changing the vehicle status" });
        }
    }

    /// <summary>
    /// Soft delete a vehicle (Admin and Planner only)
    /// </summary>
    /// <param name="id">Vehicle ID</param>
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
            await _deleteVehicle.ExecuteAsync(id);

            _logger.LogInformation(
                "Vehicle deleted: {VehicleId} by user {UserId}",
                id,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Vehicle not found for deletion: {VehicleId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while deleting vehicle: {VehicleId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle: {VehicleId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the vehicle" });
        }
    }
}
