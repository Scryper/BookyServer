namespace BookyServer.Domain.Entities;

public sealed class ProfileInterest
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsReadingInterest { get; set; }
}
