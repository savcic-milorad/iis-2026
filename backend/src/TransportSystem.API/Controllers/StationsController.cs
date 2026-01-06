using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Application.Stations.DTOs;
using TransportSystem.Application.Stations.UseCases;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.API.Controllers;

/// <summary>
/// Controller for managing stations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StationsController : ControllerBase
{
    private readonly CreateStationUseCase _createStation;
    private readonly UpdateStationUseCase _updateStation;
    private readonly DeleteStationUseCase _deleteStation;
    private readonly GetStationUseCase _getStation;
    private readonly GetAllStationsUseCase _getAllStations;
    private readonly ILogger<StationsController> _logger;

    public StationsController(
        CreateStationUseCase createStation,
        UpdateStationUseCase updateStation,
        DeleteStationUseCase deleteStation,
        GetStationUseCase getStation,
        GetAllStationsUseCase getAllStations,
        ILogger<StationsController> logger)
    {
        _createStation = createStation;
        _updateStation = updateStation;
        _deleteStation = deleteStation;
        _getStation = getStation;
        _getAllStations = getAllStations;
        _logger = logger;
    }

    /// <summary>
    /// Get all stations with optional search
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter by name, address, or description</param>
    /// <param name="includeDeleted">Whether to include soft-deleted stations (default: false)</param>
    /// <returns>List of stations</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<StationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StationResponseDto>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool includeDeleted = false)
    {
        try
        {
            var stations = await _getAllStations.ExecuteAsync(searchTerm, includeDeleted);
            return Ok(stations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stations");
            return StatusCode(500, new { message = "An error occurred while retrieving stations" });
        }
    }

    /// <summary>
    /// Get a station by ID
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <returns>Station details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StationResponseDto>> GetById(Guid id)
    {
        try
        {
            var station = await _getStation.ExecuteAsync(id);
            return Ok(station);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Station not found: {StationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving station: {StationId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the station" });
        }
    }

    /// <summary>
    /// Create a new station (Admin and Planner only)
    /// </summary>
    /// <param name="dto">Station creation data</param>
    /// <returns>Created station</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(StationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<StationResponseDto>> Create([FromBody] CreateStationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var station = await _createStation.ExecuteAsync(dto);

            _logger.LogInformation(
                "Station created: {StationId} - {StationName} by user {UserId}",
                station.Id,
                station.Name,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return CreatedAtAction(
                nameof(GetById),
                new { id = station.Id },
                station);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while creating station");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating station");
            return StatusCode(500, new { message = "An error occurred while creating the station" });
        }
    }

    /// <summary>
    /// Update an existing station (Admin and Planner only)
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <param name="dto">Station update data</param>
    /// <returns>Updated station</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Planner")]
    [ProducesResponseType(typeof(StationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StationResponseDto>> Update(Guid id, [FromBody] UpdateStationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var station = await _updateStation.ExecuteAsync(id, dto);

            _logger.LogInformation(
                "Station updated: {StationId} - {StationName} by user {UserId}",
                station.Id,
                station.Name,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return Ok(station);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Station not found for update: {StationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while updating station: {StationId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating station: {StationId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the station" });
        }
    }

    /// <summary>
    /// Soft delete a station (Admin and Planner only)
    /// </summary>
    /// <param name="id">Station ID</param>
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
            await _deleteStation.ExecuteAsync(id);

            _logger.LogInformation(
                "Station deleted: {StationId} by user {UserId}",
                id,
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Station not found for deletion: {StationId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed while deleting station: {StationId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting station: {StationId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the station" });
        }
    }
}
