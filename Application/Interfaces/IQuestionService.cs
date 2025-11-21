using Application.Result;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IQuestionService
{
    Task<OperationResult<IEnumerable<Question>>> GetAllQuestionsAsync(Game game);
    Task<OperationResult> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer);
}