using Microsoft.EntityFrameworkCore;
using myMusic.Domain.Entities;
using myMusic.Domain.Interfaces;
using myMusic.Infrastructure.Data;

namespace myMusic.Infrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly AppDbContext _context;

    public TokenRepository(AppDbContext context)
    {
        _context = context;
    }
    
    // -----------------------------------------------------
    
    // Get token by refresh token:
    public async Task<Token?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Tokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);
    }

    
    // Add token:
    public async Task<Token> AddAsync(Token token)
    {
        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    
    // Update token:
    public async Task<Token> UpdateAsync(Token token)
    {
        _context.Tokens.Update(token);
        await _context.SaveChangesAsync();
        return token;
    }

    
    // Delete by User id:
    public async Task<bool> DeleteByUserIdAsync(int userId)
    {
        var tokens = await _context.Tokens.Where(t => t.UserId == userId).ToListAsync();
        _context.Tokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
        return true;
    }

    
    // Is a valid token:
    public async Task<bool> IsValidAsync(string refreshToken, int userId)
    {
        return await _context.Tokens.AnyAsync(t =>
            t.RefreshToken == refreshToken &&
            t.UserId == userId &&
            t.RefreshTokenExpire > DateTime.UtcNow);
    }
}