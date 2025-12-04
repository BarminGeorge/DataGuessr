using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext db;

    public GameRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult> SaveStatisticAsync(Guid gameId, Statistic statistic, CancellationToken ct)
    {
        return await OperationResult.TryAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(statistic);

            var game = await db.Games
                .Include(g => g.CurrentStatistic)
                .FirstOrDefaultAsync(g => g.Id == gameId, cancellationToken: ct);

            if (game == null)
                throw new InvalidOperationException($"Game with Id '{gameId}' not found.");

            game.CurrentStatistic = statistic;
            db.Games.Update(game);

            var affectedRows = await db.SaveChangesAsync(ct);
            if (affectedRows == 0)
                throw new InvalidOperationException("Failed to save the statistic.");
        });
    }

    public async Task<OperationResult> AddGameAsync(Game game, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(game);

        return await OperationResult.TryAsync(async () =>
        {
            await db.Games.AddAsync(game, ct);
            var affectedRows = await db.SaveChangesAsync(ct);

            if (affectedRows == 0)
                throw new InvalidOperationException("Failed to add the game.");
        });
    }
}