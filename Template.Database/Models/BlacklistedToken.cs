namespace Template.Database.Models;

public class BlacklistedToken
{
    public long Id { get; set; }

    public required string Token { get; set; }
}