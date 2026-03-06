using myMusic.Domain.Entities;

namespace myMusic.Domain.Interfaces;

public interface IPlayHistoryRepository
{
    Task<IEnumerable<PlayHistory>> GetUserHistoryAsync(int userId);
    Task<PlayHistory> AddAsync(PlayHistory history);
}