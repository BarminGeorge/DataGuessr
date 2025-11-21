using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IQuestionService
{
    Task<IEnumerable<Question>> GetAllQuestionsAsync();
    void SubmitAnswerAsync(Guid questionId, Answer answer);
}