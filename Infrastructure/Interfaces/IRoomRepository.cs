using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IRoomRepository
{
    Task<OperationResult<Room>> GetByIdAsync(Guid id, CancellationToken ct);
    Task<OperationResult<IEnumerable<Room>>> GetWaitingPublicRoomsAsync(CancellationToken ct);
    Task<OperationResult> AddAsync(Room room, CancellationToken ct);
    Task<OperationResult> UpdateAsync(Room room, CancellationToken ct);
    Task<OperationResult> RemoveAsync(Guid id, CancellationToken ct);
}