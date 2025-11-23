using Application.Result;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Interfaces.Infrastructure;

public interface IQuestionRepository
{
    Task<OperationResult<IEnumerable<Question>>> GetQuestionsAsync(int count);
}