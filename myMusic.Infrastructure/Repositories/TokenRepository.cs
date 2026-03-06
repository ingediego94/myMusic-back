using System.Security.Cryptography;
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

    
    // Get token with user:
    public async Task<Token?> GetTokenWithUserAsync(string refreshToken)
    {
        return await _context.Tokens
            .Include(t => t.User)   // Esto realiza un JOIN a Users<
            .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);
    }


    // Add token:
    public async Task<Token> AddAsync(Token token)
    {
        token.CreatedAt = DateTime.UtcNow;      // asegurar fechas desde el repo
        token.UpdatedAt = DateTime.UtcNow;
        
        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    
    // Update token:
    public async Task<Token> UpdateAsync(Token token)
    {
        token.UpdatedAt = DateTime.UtcNow;
        
        _context.Tokens.Update(token);
        await _context.SaveChangesAsync();
        return token;
    }

    
    // Delete by refresh token: (borrar token en una sola sesion).
    public async Task<bool> DeleteAsync(string refreshToken)
    {
        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);
        if (token == null) return false;

        _context.Tokens.Remove(token);
        return await _context.SaveChangesAsync() > 0;
        // Retorna la cantidad de filas afectadas, si es >0 entonces fue exitoso.
    }

    
    // Delete all by user id: (cerrar todas las sesiones).
    public async Task<bool> DeleteAllByUserIdAsync(int userId)
    {
        var deleteRows = await _context.Tokens
            .Where(t => t.UserId == userId)
            .ExecuteDeleteAsync();

        return deleteRows > 0;
    }


    
    
    
    
    
    // // Delete by User id:
    // public async Task<bool> DeleteByUserIdAsync(int userId)
    // {
    //     var tokens = await _context.Tokens.Where(t => t.UserId == userId).ToListAsync();
    //     _context.Tokens.RemoveRange(tokens);
    //     await _context.SaveChangesAsync();
    //     return true;
    // }
    //
    //
    // // Is a valid token:
    // public async Task<bool> IsValidAsync(string refreshToken, int userId)
    // {
    //     return await _context.Tokens.AnyAsync(t =>
    //         t.RefreshToken == refreshToken &&
    //         t.UserId == userId &&
    //         t.RefreshTokenExpire > DateTime.UtcNow);
    // }
}