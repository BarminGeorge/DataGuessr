using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext db;

    public QuestionRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<IEnumerable<Question>>> GetUniqQuestionsAsync(int count, CancellationToken ct)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero.");

        return await OperationResult<IEnumerable<Question>>.TryAsync(async () =>
        {
            var allIds = await db.Questions
                .AsNoTracking()
                .Select(q => q.Id)
                .ToListAsync(ct);

            if (allIds.Count == 0)
                throw new InvalidOperationException("No questions found in the database.");

            if (count > allIds.Count)
                count = allIds.Count;
            
            var random = new Random();
            var selectedIds = allIds.OrderBy(_ => random.Next()).Take(count).ToList();
            
            var questions = await db.Questions
                .AsNoTracking()
                .Where(q => selectedIds.Contains(q.Id))
                .ToListAsync(ct);

            return questions;
        });
    }
}