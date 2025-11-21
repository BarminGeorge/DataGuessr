using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IGameRepository
{
    Task SaveAnswerAsync(Guid questionId, Answer answer);
}