using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;

namespace Infrastructure.PostgreSQL.Repositories;

public class GameRepository : IGameRepository
{
    public Task<OperationResult> SaveStatisticAsync(Guid gameId, Statistic statistic, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> AddGameAsync(Game game, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}