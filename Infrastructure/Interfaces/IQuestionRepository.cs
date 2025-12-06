using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Interfaces;

public interface IQuestionRepository
{
    Task<OperationResult<IEnumerable<Question>>> GetUniqQuestionsAsync(int count, GameMode mode, CancellationToken ct);
}