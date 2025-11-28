using Application.Result;

namespace Application.Interfaces.Infrastructure;

public interface IConnectionRepository
{
    Task<OperationResult<(Guid userId, Guid roomId)>> GetUserByConnectionIdAsync(string connectionId);
    Task<OperationResult> AddConnectionAsync(string connectionId, Guid userId, Guid roomId, CancellationToken ct);
    Task<OperationResult> RemoveConnectionAsync(string connectionId);
}