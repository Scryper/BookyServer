namespace BookyServer.Domain.Entities;

public sealed class Profile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool HasVehicle { get; set; }
    public bool OffersCarpool { get; set; }
    public bool SeeksCarpool { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ProfileInterest> Interests { get; set; } = new List<ProfileInterest>();
    public ICollection<ProfileBook> Books { get; set; } = new List<ProfileBook>();
    public ICollection<ProfilePhoto> Photos { get; set; } = new List<ProfilePhoto>();
}
