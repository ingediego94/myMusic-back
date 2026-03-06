using Microsoft.EntityFrameworkCore;
using myMusic.Domain.Entities;
using myMusic.Domain.Interfaces;
using myMusic.Infrastructure.Data;

namespace myMusic.Infrastructure.Repositories;

public class PlaylistRepository : IPlaylistRespository
{
    private readonly AppDbContext _context;

    public PlaylistRepository(AppDbContext context)
    {
        _context = context;
    }

    // -----------------------------------------------------------
    
    // Get all playlists:
    public async Task<IEnumerable<Playlist>> GetAllPlaylistAsync()
    {
        return await _context.Playlists.ToListAsync();
    }

    
    // Get playlists by user id:
    public async Task<IEnumerable<Playlist>> GetByUserIdAsync(int userId)
    {
        return await _context.Playlists
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }
    

    // Get playlist by id with songs:
    public async Task<Playlist?> GetByIdWithSongsAsync(int id)
    {
        return await _context.Playlists
            .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    
    // Create a playlist:
    public async Task<Playlist> CreatePlaylistAsync(Playlist playlist)
    {
        playlist.CreatedAt = DateTime.UtcNow;
        playlist.UpdatedAt = DateTime.UtcNow;

        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();
        return playlist;
    }

    
    
    // Update a playlist:
    public async Task<Playlist> UpdatePlaylistAsync(Playlist playlist)
    {
        playlist.UpdatedAt = DateTime.UtcNow;

        _context.Playlists.Update(playlist);
        await _context.SaveChangesAsync();
        return playlist;
    }

    
    // Delete a playlist:
    public async Task<bool> DeletePlaylistAsync(Playlist playlist)
    {
        return await _context.Playlists
            .Where(p => p.Id == playlist.Id)
            .ExecuteDeleteAsync() > 0;
    }
}