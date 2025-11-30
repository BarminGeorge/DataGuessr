using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;

namespace Infrastructure.Interfaces;

public interface IGameRepository
{
    Task<OperationResult> SaveStatisticAsync(Guid gameId, Statistic statistic, CancellationToken ct);
    Task<OperationResult> AddGameAsync(Game game, CancellationToken ct);
}