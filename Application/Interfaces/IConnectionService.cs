using Application.Result;

namespace Application.Interfaces;

public interface IConnectionService
{
    Task<OperationResult> AddConnection(string connectionId, Guid userId, Guid roomId, CancellationToken ct);
    Task<OperationResult> RemoveConnection(string connectionId);
    Task<OperationResult<(Guid userId, Guid roomId)>> GetUserByConnection(string connectionId);
}