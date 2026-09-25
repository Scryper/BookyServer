namespace BookyServer.Interfaces.Contracts;

public sealed record RateProfileBookRequest(string Rating, DateOnly? ReadAt);
