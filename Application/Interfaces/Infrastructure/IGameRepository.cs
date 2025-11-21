using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IGameRepository
{
    Task SaveAnswerAsync(Guid questionId, Answer answer);
    Task<Dictionary<Guid, Answer>> LoadAnswersAsync(Guid questionId);
    Task SaveStatisticAsync(Statistic statistic);
    Task<Statistic> LoadCurrentStatisticAsync(Guid gameId);
}