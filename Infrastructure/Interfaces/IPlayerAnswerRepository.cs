using Domain.Common;
using Domain.ValueTypes;

namespace Infrastructure.Interfaces;

public interface IPlayerAnswerRepository
{
    Task<OperationResult> SaveAnswerAsync(Guid gameId, Guid questionId, Guid playerId, Answer answer, CancellationToken ct);

    Task<OperationResult<Dictionary<Guid, Answer>>> LoadAnswersAsync(Guid gameId, Guid questionId, CancellationToken ct);
}
