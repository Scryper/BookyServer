namespace BookyServer.Api.Helpers.Models;

public sealed class ErrorResponse
{
    public int ErrorCode { get; set; }
    public string Description { get; set; } = string.Empty;
}
