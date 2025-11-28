using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Result;

namespace Application.Services;

public class ConnectionService(IConnectionRepository connectionRepository) : IConnectionService
{
    public async Task<OperationResult> AddConnection(string connectionId, Guid userId, Guid roomId, CancellationToken ct)
    {
        return await connectionRepository.AddConnectionAsync(connectionId, userId, roomId, ct);
    }

    public async Task<OperationResult> RemoveConnection(string connectionId)
    {
        return await connectionRepository.RemoveConnectionAsync(connectionId);
    }

    public async Task<OperationResult<(Guid userId, Guid roomId)>> GetUserByConnection(string connectionId)
    {
        return await connectionRepository.GetUserByConnectionIdAsync(connectionId);
    }
}