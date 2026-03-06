using myMusic.Domain.Entities;

namespace myMusic.Domain.Interfaces;

public interface IPlaylist
{
    Task<IEnumerable<Playlist>> GetAllPlaylistAsync();
    Task<IEnumerable<Playlist>> GetByUserIdAsync(int userId);
    Task<Playlist?> GetByIdWithSongsAsync(int id);
    Task<Playlist> CreatePlaylistAsync(Playlist playlist);
    Task<Playlist> UpdatePlaylistAsync(Playlist playlist);
    Task<bool> DeletePlaylistAsync(Playlist playlist);
}
