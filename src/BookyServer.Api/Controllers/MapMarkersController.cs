using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Route("api/v1/map-markers")]
public sealed class MapMarkersController(IMapMarkerService markers) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<IReadOnlyList<MapMarkerDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublished(CancellationToken cancellationToken) =>
        Ok(await markers.GetPublishedAsync(cancellationToken));

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType<MapMarkerDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateMapMarkerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var marker = await markers.CreateAsync(request, cancellationToken);
            return Created("/api/v1/map-markers", marker);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Lieu invalide",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
