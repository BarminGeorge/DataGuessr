using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Result;

namespace Application.Services;

public class ConnectionService(IConnectionRepository connectionRepository) : IConnectionService
{
    public async Task<OperationResult> AddConnection(string connectionId, Guid playerId, Guid roomId, CancellationToken ct)
    {
        return await connectionRepository.AddConnectionAsync(connectionId, playerId, roomId, ct);
    }

    public async Task<OperationResult> RemoveConnection(string connectionId)
    {
        return await connectionRepository.RemoveConnectionAsync(connectionId);
    }

    public async Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnection(string connectionId)
    {
        return await connectionRepository.GetPlayerByConnectionIdAsync(connectionId);
    }
}