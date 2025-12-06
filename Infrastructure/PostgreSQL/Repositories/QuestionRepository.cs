using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public QuestionRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<IEnumerable<Question>>> GetUniqQuestionsAsync(int count, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<IEnumerable<Question>>>>(async () =>
        {
            if (count <= 0)
                return OperationResult<IEnumerable<Question>>.Error.Validation("Количество вопросов должно быть больше нуля");

            var allIds = await db.Questions
                .AsNoTracking()
                .Select(q => q.Id)
                .ToListAsync(ct);

            if (allIds.Count == 0)
                return OperationResult<IEnumerable<Question>>.Error.NotFound("Вопросы не найдены в базе данных");

            if (count > allIds.Count)
                count = allIds.Count;

            var random = new Random();
            var selectedIds = allIds.OrderBy(_ => random.Next()).Take(count).ToList();

            var questions = await db.Questions
                .AsNoTracking()
                .Where(q => selectedIds.Contains(q.Id))
                .ToListAsync(ct);

            return OperationResult<IEnumerable<Question>>.Ok(questions);

        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }
}