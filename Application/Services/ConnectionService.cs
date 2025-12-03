using Application.Interfaces;
using Domain.Common;
using Infrastructure.Interfaces;

namespace Application.Services;

public class ConnectionService(IPlayerRepository playerRepository) : IConnectionService
{
    public async Task<OperationResult> AddConnection(string connectionId, Guid playerId, Guid roomId, CancellationToken ct)
    {
        return await playerRepository.CreatePlayerAsync(connectionId, playerId, roomId, ct);
    }

    public async Task<OperationResult> RemoveConnection(string connectionId)
    {
        return await playerRepository.RemovePlayerByConnectionAsync(connectionId);
    }

    public async Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnection(string connectionId)
    {
        return await playerRepository.GetPlayerByConnectionIdAsync(connectionId);
    }

    public async Task<OperationResult<string>> GetConnectionIdByPlayer(Guid playerId)
    {
        return await playerRepository.GetConnectionByPlayerAsync(playerId);
    }
}