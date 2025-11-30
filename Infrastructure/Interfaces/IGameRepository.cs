using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IGameRepository
{
    Task<OperationResult> SaveStatisticAsync(Guid gameId, Statistic statistic, CancellationToken ct);
    Task<OperationResult> AddGameAsync(Game game, CancellationToken ct);
}