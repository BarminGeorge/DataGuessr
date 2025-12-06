using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public GameRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult> SaveStatisticAsync(Guid gameId, Statistic statistic, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (gameId == Guid.Empty)
                return OperationResult.Error.Validation("GameId не может быть пустым GUID");

            if (statistic == null)
                return OperationResult.Error.Validation("Статистика не может быть null");

            var game = await db.Games
                .Include(g => g.CurrentStatistic)
                .FirstOrDefaultAsync(g => g.Id == gameId, cancellationToken: ct);

            if (game == null)
                return OperationResult.Error.NotFound($"Игра с ID '{gameId}' не найдена");

            game.CurrentStatistic = statistic;
            db.Games.Update(game);

            var affectedRows = await db.SaveChangesAsync(ct);
            if (affectedRows == 0)
                return OperationResult.Error.InternalError("Не удалось сохранить статистику");

            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }

    public async Task<OperationResult> AddGameAsync(Game game, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (game == null)
                return OperationResult.Error.Validation("Игра не может быть null");

            if (game.Id == Guid.Empty)
                return OperationResult.Error.Validation("Id игры не может быть пустым GUID");

            await db.Games.AddAsync(game, ct);
            var affectedRows = await db.SaveChangesAsync(ct);

            if (affectedRows == 0)
                return OperationResult.Error.InternalError("Не удалось добавить игру");

            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }
}