using myMusic.Domain.Entities;

namespace myMusic.Domain.Interfaces;

public interface IPlaylistSong
{
    Task<bool> AddSongToPlaylistAsync(PlaylistSong playlistSong);
    Task<bool> RemoveSongFromPlaylistAsync(int playlistId, int songId);
    Task<bool> IsSongInPlaylistAsync(int playlistId, int songId);
}