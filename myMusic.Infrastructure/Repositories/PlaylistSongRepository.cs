using Microsoft.EntityFrameworkCore;
using myMusic.Domain.Entities;
using myMusic.Domain.Interfaces;
using myMusic.Infrastructure.Data;

namespace myMusic.Infrastructure.Repositories;

public class PlaylistSongRepository : IPlaylistSongRepository
{
    private readonly AppDbContext _context;

    public PlaylistSongRepository(AppDbContext context)
    {
        _context = context;
    }
    
    // --------------------------------------------------------
    
    // Check if a song is in a playlist:
    public async Task<bool> IsSongInPlaylistAsync(int playlistId, int songId)
    {
        return await _context.PlaylistSongs
            .AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
    }
    
    
    // Add a new song to a playlist:
    public async Task<bool> AddSongToPlaylistAsync(PlaylistSong playlistSong)
    {
        var exists = await IsSongInPlaylistAsync(playlistSong.PlaylistId, playlistSong.SongId);
        if (exists) return false;
        
        playlistSong.CreatedAt = DateTime.UtcNow;
        playlistSong.UpdatedAt = DateTime.UtcNow;

        _context.PlaylistSongs.Add(playlistSong);
        return await _context.SaveChangesAsync() > 0;
    }
    
    
    // Remove a song of a playlist:
    public async Task<bool> RemoveSongFromPlaylistAsync(int playlistId, int songId)
    {
        return await _context.PlaylistSongs
            .Where(ps => ps.PlaylistId == playlistId && ps.SongId == songId)
            .ExecuteDeleteAsync() > 0;
    }
}