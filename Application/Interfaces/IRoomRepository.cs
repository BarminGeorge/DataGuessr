using Domain.Entities;

namespace Application.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<IEnumerable<Room>> GetRoomsAsync();
    Task<IEnumerable<Room>?> GetWaitingPublicRoomsAsync();
    Task AddAsync(Room room);
    Task UpdateAsync(Room room);
    Task RemoveAsync(Guid id);
    Task<Game> AddGameAsync(Game game);
    Task<Game> UpdateGameAsync(Game game);
}