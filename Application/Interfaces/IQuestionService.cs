using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IQuestionService
{
    Task<OperationResult<IEnumerable<Question>>> GetAllQuestionsAsync(Game game, CancellationToken ct);
    Task<OperationResult> SubmitAnswerAsync(Guid gameId, Guid questionId, Guid playerId, Answer answer, CancellationToken ct);
}