using Domain.Common;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IQuestionRepository
{
    Task<OperationResult<IEnumerable<Question>>> GetUniqQuestionsAsync(int count, CancellationToken ct);
}