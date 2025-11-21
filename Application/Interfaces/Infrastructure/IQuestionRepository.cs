using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetQuestionsAsync(int count);
}