using Domain.Common;
using Domain.ValueTypes;
using Infrastructure.Interfaces;

namespace Infrastructure.PostgreSQL.Repositories;

public class PlayerAnswerRepository : IPlayerAnswerRepository
{
    public Task<OperationResult> SaveAnswerAsync(Guid gameId, Guid questionId, Guid playerId, Answer answer, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<Dictionary<Guid, Answer>>> LoadAnswersAsync(Guid gameId, Guid questionId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}