using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Route(Constants.Routes.MapMarkers)]
public sealed class MapMarkersController(IMapMarkerService markers) : ControllerBase
{
    private readonly IMapMarkerService _markers = markers ?? throw new ArgumentNullException(nameof(markers));

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<IReadOnlyList<MapMarkerDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublishedAsync(CancellationToken cancellationToken)
    {
        return Ok(await this._markers.GetPublishedAsync(cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType<MapMarkerDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        CreateMapMarkerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var marker = await this._markers.CreateAsync(request, cancellationToken);
            return Created(Constants.Routes.MapMarkersPath, marker);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = Constants.Errors.InvalidMapMarker,
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
