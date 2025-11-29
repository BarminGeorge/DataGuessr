using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IGameRepository
{
    Task<OperationResult> SaveAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer, CancellationToken ct);
    Task<OperationResult<Dictionary<Guid, Answer>>> LoadAnswersAsync(Guid questionId, CancellationToken ct);
    Task<OperationResult> SaveStatisticAsync(Statistic statistic, CancellationToken ct);
    Task<OperationResult> AddGameAsync(Game game, CancellationToken ct);
    Task<OperationResult<Game>?> GetCurrentGameAsync(Guid roomId, CancellationToken ct);
}