using Application.Result;
using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IGameRepository
{
    Task<OperationResult> SaveAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer, CancellationToken ct);
    Task<OperationResult<Dictionary<Guid, Answer>>> LoadAnswersAsync(Guid questionId, CancellationToken ct);
    Task<OperationResult> SaveStatisticAsync(Statistic statistic, CancellationToken ct);
    Task<OperationResult<Statistic>> LoadCurrentStatisticAsync(Guid gameId, CancellationToken ct);
}