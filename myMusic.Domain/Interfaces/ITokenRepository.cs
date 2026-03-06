using myMusic.Domain.Entities;

namespace myMusic.Domain.Interfaces;

public interface ITokenRepository
{
    Task<Token?> GetByRefreshTokenAsync(string refreshToken);
    Task<Token?> GetTokenWithUserAsync(string refreshToken);
    Task<Token> AddAsync(Token token);
    Task<Token> UpdateAsync(Token token);
    Task<bool> DeleteAsync(string refreshToken);    // Para borrar un token específico de una sola sesion
    Task<bool> DeleteAllByUserIdAsync(int userId);  // Para "Cerrar todas las sesiones"
    
    // Task<bool> DeleteByUserIdAsync(int userId);
    // Task<bool> IsValidAsync(string refreshToken, int userId);
}
