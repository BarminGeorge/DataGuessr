using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class PlayerAnswerRepository : IPlayerAnswerRepository
{
    private readonly AppDbContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public PlayerAnswerRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult> SaveAnswerAsync(Guid gameId, Guid questionId, Guid playerId, Answer answer,
        CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (gameId == Guid.Empty)
                return OperationResult.Error.Validation("GameId не может быть пустым GUID");

            if (questionId == Guid.Empty)
                return OperationResult.Error.Validation("QuestionId не может быть пустым GUID");

            if (playerId == Guid.Empty)
                return OperationResult.Error.Validation("PlayerId не может быть пустым GUID");

            if (answer == null)
                return OperationResult.Error.Validation("Ответ не может быть null");

            await db.PlayerAnswers.AddAsync(new PlayerAnswer(gameId, playerId, questionId, answer), ct);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }

    public async Task<OperationResult<Dictionary<Guid, Answer>>> LoadAnswersAsync(Guid gameId, Guid questionId,
        CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<Dictionary<Guid, Answer>>>>(async () =>
        {
            if (gameId == Guid.Empty)
                return OperationResult<Dictionary<Guid, Answer>>.Error.Validation("GameId не может быть пустым GUID");

            if (questionId == Guid.Empty)
                return OperationResult<Dictionary<Guid, Answer>>.Error.Validation("QuestionId не может быть пустым GUID");

            var answers = await db.PlayerAnswers
                .AsNoTracking()
                .Where(answer => answer.GameId == gameId && answer.QuestionId == questionId)
                .ToListAsync(ct);

            var result = answers.ToDictionary(answer => answer.PlayerId, answer => answer.Answer);

            return OperationResult<Dictionary<Guid, Answer>>.Ok(result);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }
}