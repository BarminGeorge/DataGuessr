using Application.Result;
using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IGameRepository
{
    Task<OperationResult> SaveAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer);
    Task<OperationResult<Dictionary<Guid, Answer>>> LoadAnswersAsync(Guid questionId);
    Task<OperationResult> SaveStatisticAsync(Statistic statistic);
    Task<OperationResult<Statistic>> LoadCurrentStatisticAsync(Guid gameId);
}