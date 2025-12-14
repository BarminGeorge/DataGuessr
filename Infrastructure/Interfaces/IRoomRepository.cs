using Domain.Common;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IRoomRepository
{
    Task<OperationResult<Room>> GetByIdAsync(Guid id, CancellationToken ct);
    Task<OperationResult<IEnumerable<Room>>> GetWaitingPublicRoomsAsync(CancellationToken ct);
    Task<OperationResult> AddAsync(Room room, CancellationToken ct);
    Task<OperationResult> UpdateAsync(Room room, CancellationToken ct);
    Task<OperationResult<Game>> GetCurrentGameAsync(Guid roomId, CancellationToken ct);
}