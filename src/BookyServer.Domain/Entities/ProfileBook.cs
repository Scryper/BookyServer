namespace BookyServer.Domain.Entities;

public sealed class ProfileBook
{
    public Guid ProfileId { get; set; }
    public Guid BookId { get; set; }
    public BookRating Rating { get; set; }
    public DateOnly? ReadAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Profile Profile { get; set; } = null!;
    public Book Book { get; set; } = null!;
}
