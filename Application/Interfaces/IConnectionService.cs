using Domain.Common;

namespace Application.Interfaces;

public interface IConnectionService
{
    Task<OperationResult> AddConnection(string connectionId, Guid userId, Guid roomId, CancellationToken ct);
    Task<OperationResult> RemoveConnection(string connectionId);
    Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnection(string connectionId);
    Task<OperationResult<string>> GetConnectionIdByPlayer(Guid playerId);
}