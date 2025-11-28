using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;

namespace Infrastructure.Interfaces;

public interface IPlayerRepository
{
    Task<OperationResult<Player>> GetPlayerByIdAsync(Guid playerId, CancellationToken ct = default);
    Task<OperationResult<Player>> CreatePlayerAsync(User user, CancellationToken ct = default);
    Task<OperationResult> UpdatePlayerScoreAsync(Guid playerId, Score newScore, CancellationToken ct = default);
    Task<OperationResult> DeletePlayerAsync(Guid playerId, CancellationToken ct = default);
}