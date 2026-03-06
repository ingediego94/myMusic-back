using Microsoft.EntityFrameworkCore;
using myMusic.Domain.Entities;
using myMusic.Domain.Interfaces;
using myMusic.Infrastructure.Data;

namespace myMusic.Infrastructure.Repositories;

public class PlayHistoryRepository : IPlayHistoryRepository
{
    private readonly AppDbContext _context;

    public PlayHistoryRepository(AppDbContext context)
    {
        _context = context;
    }
    
    // -------------------------------------------------------------
    
    // Get user history:
    public async Task<IEnumerable<PlayHistory>> GetUserHistoryAsync(int userId)
    {
        return await _context.PlayHistories
            .Include(ph => ph.Song)
            .Where(ph => ph.UserId == userId)
            .OrderByDescending(ph => ph.PlayedAt)
            .ToListAsync();
    }

    public async Task<PlayHistory> AddAsync(PlayHistory history)
    {
        history.PlayedAt = DateTime.UtcNow;
        _context.PlayHistories.Add(history);
        await _context.SaveChangesAsync();
        return history;
    }
}