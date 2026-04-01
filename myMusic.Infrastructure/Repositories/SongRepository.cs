using Microsoft.EntityFrameworkCore;
using myMusic.Domain.Entities;
using myMusic.Domain.Interfaces;
using myMusic.Infrastructure.Data;

namespace myMusic.Infrastructure.Repositories;

public class SongRepository : ISongRepository
{
    private readonly AppDbContext _context;

    public SongRepository( AppDbContext context)
    {
        _context = context;
    }
    
    // -----------------------------------------
    
    // Get all songs:
    public async Task<IEnumerable<Song>> GetAllAsync()
    {
        return await _context.Songs.ToListAsync();
    }

    
    // Get song by id:
    public async Task<Song?> GetByIdAsync(int id)
    {
        return await _context.Songs.FindAsync(id);
    }

    
    // Add song:
    public async Task<Song> AddAsync(Song song)
    {
        _context.Songs.Add(song);
        await _context.SaveChangesAsync();
        return song;
    }

    
    // Update song:
    public async Task<Song?> UpdateAsync(Song song)
    {
        _context.Songs.Update(song);
        await _context.SaveChangesAsync();
        return song;
    }

    
    // Delete song:
    public async Task<bool> DeleteAsync(Song song)
    {
        _context.Songs.Remove(song);
        return await _context.SaveChangesAsync() > 0;
    }
}