using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IQuestionService
{
    Task<IEnumerable<Question>> GetAllQuestionsAsync(Game game);
    Task SubmitAnswerAsync(Guid questionId, Answer answer);
}