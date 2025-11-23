using Application.Result;
using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IRoomRepository
{
    Task<OperationResult<Room>> GetByIdAsync(Guid id);
    Task<OperationResult<IEnumerable<Room>>> GetRoomsAsync();
    Task<OperationResult<IEnumerable<Room>>> GetWaitingPublicRoomsAsync();
    Task<OperationResult> AddAsync(Room room);
    Task<OperationResult> UpdateAsync(Room room);
    Task<OperationResult> RemoveAsync(Guid id);
    Task<OperationResult<Game>> AddGameAsync(Game game);
    Task<OperationResult<Game>?> GetCurrentGameAsync(Guid roomId);
}