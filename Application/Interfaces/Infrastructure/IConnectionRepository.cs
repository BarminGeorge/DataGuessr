using Application.Result;

namespace Application.Interfaces.Infrastructure;

public interface IConnectionRepository
{
    Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnectionIdAsync(string connectionId);
    Task<OperationResult> AddConnectionAsync(string connectionId, Guid playerId, Guid roomId, CancellationToken ct);
    Task<OperationResult> RemoveConnectionAsync(string connectionId);
}