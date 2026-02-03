using myMusic.Domain.Entities;

namespace myMusic.Domain.Interfaces;

public interface ISongRepository
{
    Task<IEnumerable<Song>> GetAllAsync();
    Task<Song?> GetByIdAsync(int id);
    Task<Song> AddAsync(Song song);
    Task<Song?> UpdateAsync(Song song);
    Task<bool?> DeleteAsync(Song song);
}