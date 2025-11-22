using Application.Result;

namespace Application.Interfaces.Infrastructure;

public interface IConnectionRepository
{
    Task<OperationResult<(Guid userId, Guid roomId)>> GetUserByConnectionId(string connectionId);
    Task<OperationResult> AddConnection(string connectionId, Guid userId, Guid roomId);
    Task<OperationResult> RemoveConnection(string connectionId);
}