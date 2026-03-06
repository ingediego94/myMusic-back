using myMusic.Domain.Entities;

namespace myMusic.Domain.Interfaces;

public interface IPlayHistory
{
    Task<IEnumerable<PlayHistory>> GetUserHistoryAsync(int userId);
    Task<PlayHistory> AddAsync(PlayHistory history);
}