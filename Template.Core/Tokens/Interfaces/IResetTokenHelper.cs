namespace Template.Core.Tokens.Interfaces;

public interface IResetTokenHelper
{
    (string Token, DateTime ExpiredAt) GenerateResetToken(Guid userId);
    Guid ReadToken(string token);
}