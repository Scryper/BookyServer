using BookyServer.Infrastructure.Persistence;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("health")]
public sealed class HealthController(BookyServerDbContext db) : ControllerBase
{
    private const string LiveStatus = "live";
    private const string ReadyStatus = "ready";

    private readonly BookyServerDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    [HttpGet("live")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Live()
    {
        return this.Ok(new { status = LiveStatus });
    }

    [HttpGet("ready")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ReadyAsync(CancellationToken cancellationToken)
    {
        var canConnect = await this._db.Database.CanConnectAsync(cancellationToken);
        return canConnect
            ? this.Ok(new { status = ReadyStatus })
            : this.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
}
