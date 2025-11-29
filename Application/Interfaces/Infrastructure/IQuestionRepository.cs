using Application.Result;
using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IQuestionRepository
{
    Task<OperationResult<IEnumerable<Question>>> GetUniqQuestionsAsync(int count, CancellationToken ct);
}