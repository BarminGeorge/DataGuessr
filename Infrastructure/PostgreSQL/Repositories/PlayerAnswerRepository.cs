using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class PlayerAnswerRepository : IPlayerAnswerRepository
{
    private readonly AppDbContext db;

    public PlayerAnswerRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult> SaveAnswerAsync(Guid gameId, Guid questionId, Guid playerId, Answer answer,
        CancellationToken ct)
    {
        return await OperationResult.TryAsync(async () =>
        {
            await db.PlayerAnswers.AddAsync(new PlayerAnswer(gameId, playerId, questionId, answer), ct);
            await db.SaveChangesAsync(ct);
        });
    }

    public async Task<OperationResult<Dictionary<Guid, Answer>>> LoadAnswersAsync(Guid gameId, Guid questionId,
        CancellationToken ct)
    {
        return await OperationResult<Dictionary<Guid, Answer>>.TryAsync(() =>
        {
            var answers = db.PlayerAnswers
                .AsNoTracking()
                .Where(answer => answer.GameId == gameId && answer.QuestionId == questionId);
            return Task.FromResult(answers.ToDictionary(answer => answer.QuestionId, answer => answer.Answer));
        });
    }
}