using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IPlayerRepository
{
    Task<OperationResult<Player>> GetPlayerByIdAsync(Guid playerId, CancellationToken ct = default);
    Task<OperationResult<Player>> CreatePlayerAsync(Guid userId, Guid roomId, CancellationToken ct = default);
    Task<OperationResult> UpdatePlayerScoreAsync(Guid playerId, Score newScore, CancellationToken ct = default);
    Task<OperationResult> DeletePlayerAsync(Guid playerId, CancellationToken ct = default);
    Task<OperationResult<Player>> GetPlayerByUserIdAndRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default);
    Task<OperationResult<List<Player>>> GetPlayersByRoomAsync(Guid roomId, CancellationToken ct = default);
}