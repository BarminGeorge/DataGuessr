using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Infrastructure.PostgreSQL.Repositories;

public class QuestionRepository : IQuestionRepository
{
    public Task<OperationResult<IEnumerable<Question>>> GetUniqQuestionsAsync(int count, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}