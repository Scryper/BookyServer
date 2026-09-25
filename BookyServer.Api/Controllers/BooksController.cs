using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Route("api/v1/books")]
public sealed class BooksController(IProfileService profiles) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<IReadOnlyList<BookCatalogDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? query,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default) =>
        Ok(await profiles.SearchBooksAsync(query, limit, cancellationToken));
}
