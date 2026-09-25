using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Route(Constants.Routes.Books)]
public sealed class BooksController(IProfileService profiles) : ControllerBase
{
    private readonly IProfileService _profiles = profiles ?? throw new ArgumentNullException(nameof(profiles));

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<IReadOnlyList<BookCatalogDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] string? query,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await this._profiles.SearchBooksAsync(query, limit, cancellationToken));
    }
}
