namespace Institut.Helpers.Models;

public class ErrorResponse
{
	public int ErrorCode { get; set; }
	public string Description { get; set; } = string.Empty;
}