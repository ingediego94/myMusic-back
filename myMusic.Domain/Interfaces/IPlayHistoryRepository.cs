using myMusic.Domain.Entities;

namespace myMusic.Domain.Interfaces;

public interface IPlayHistoryRepository
{
    Task<IEnumerable<PlayHistory>> GetUserHistoryAsync(int userId, int limit = 100);
    Task<PlayHistory> AddAsync(PlayHistory history);
}