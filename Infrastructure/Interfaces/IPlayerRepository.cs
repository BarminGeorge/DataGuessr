using Domain.Common;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IPlayerRepository
{
    Task<OperationResult<Player>> GetPlayerByIdAsync(Guid playerId, CancellationToken ct);
    Task<OperationResult> DeletePlayerAsync(Guid playerId, CancellationToken ct = default);
    Task<OperationResult<Player>> GetPlayerByUserIdAndRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default);
    Task<OperationResult<List<Player>>> GetPlayersByRoomAsync(Guid roomId, CancellationToken ct = default);
    Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnectionIdAsync(string connectionId);
    Task<OperationResult> CreatePlayerAsync(string connectionId, Guid userId, Guid roomId, CancellationToken ct);
    Task<OperationResult> RemovePlayerByConnectionAsync(string connectionId);
}