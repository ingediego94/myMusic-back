using myMusic.Domain.Entities;

namespace myMusic.Domain.Interfaces;

public interface ITokenRepository
{
    Task<Token?> GetByRefreshTokenAsync(string refreshToken);
    Task<Token> AddAsync(Token token);
    Task<Token> UpdateAsync(Token token);
    Task<bool> DeleteByUserIdAsync(int userId);
    Task<bool> IsValidAsync(string refreshToken, int userId);
}