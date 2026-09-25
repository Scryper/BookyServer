namespace BookyServer.Domain.Entities;

public sealed class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int? PublicationYear { get; set; }
    public string? Genre { get; set; }
    public string? Synopsis { get; set; }
    public string? Isbn13 { get; set; }
    public string? CoverUrl { get; set; }
    public string? Source { get; set; }
    public string? ExternalId { get; set; }
    public ICollection<ProfileBook> Profiles { get; set; } = new List<ProfileBook>();
}
