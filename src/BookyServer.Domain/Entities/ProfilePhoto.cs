namespace BookyServer.Domain.Entities;

public sealed class ProfilePhoto
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public PhotoCategory Category { get; set; }
    public string ObjectKey { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public enum PhotoCategory
{
    Portrait = 1,
    Library = 2
}
