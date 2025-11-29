using Application.Result;

namespace Application.Interfaces;

public interface IConnectionService
{
    Task<OperationResult> AddConnection(string connectionId, Guid playerId, Guid roomId, CancellationToken ct);
    Task<OperationResult> RemoveConnection(string connectionId);
    Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnection(string connectionId);
}