using Domain.Entities;
using Domain.Common;
using Domain.ValueTypes;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Infrastructure;

namespace Infrastructure.PostgreSQL.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _db;

    public PlayerRepository(AppDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<Player>> GetPlayerByIdAsync(Guid playerId, CancellationToken ct = default)
    {
        try
        {
            var player = await _db.Players
                .AsNoTracking()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == playerId, ct);

            if (player == null)
                return OperationResult<Player>.Error($"Игрок с ID '{playerId}' не найден");

            return OperationResult<Player>.Ok(player);
        }
        catch (OperationCanceledException)
        {
            return OperationResult<Player>.Error("Операция была отменена");
        }
        catch (Exception ex)
        {
            return OperationResult<Player>.Error($"Ошибка при получении игрока: {ex.Message}");
        }
    }

    public async Task<OperationResult<Player>> CreatePlayerAsync(User user, CancellationToken ct = default)
    {
        try
        {
            var existingPlayer = await _db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == user.Id, ct);

            if (existingPlayer != null)
                return OperationResult<Player>.Error($"Игрок для пользователя '{user.PlayerName}' уже существует");

            var player = new Player(user);
            await _db.Players.AddAsync(player, ct);
            await _db.SaveChangesAsync(ct);

            return OperationResult<Player>.Ok(player);
        }
        catch (OperationCanceledException)
        {
            return OperationResult<Player>.Error("Операция была отменена");
        }
        catch (DbUpdateException ex)
        {
            return OperationResult<Player>.Error($"Ошибка при создании игрока: {ex.Message}");
        }
        catch (Exception ex)
        {
            return OperationResult<Player>.Error($"Неожиданная ошибка: {ex.Message}");
        }
    }

    public async Task<OperationResult> UpdatePlayerScoreAsync(Guid playerId, Score newScore, CancellationToken ct = default)
    {
        try
        {
            var player = await _db.Players
                .FirstOrDefaultAsync(p => p.Id == playerId, ct);

            if (player == null)
                return OperationResult.Error($"Игрок с ID '{playerId}' не найден");

            player.UpdateScore(newScore);
            _db.Players.Update(player);
            await _db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        }
        catch (OperationCanceledException)
        {
            return OperationResult.Error("Операция была отменена");
        }
        catch (DbUpdateException ex)
        {
            return OperationResult.Error($"Ошибка при обновлении счета: {ex.Message}");
        }
        catch (Exception ex)
        {
            return OperationResult.Error($"Неожиданная ошибка: {ex.Message}");
        }
    }

    public async Task<OperationResult> DeletePlayerAsync(Guid playerId, CancellationToken ct = default)
    {
        try
        {
            var player = await _db.Players
                .FirstOrDefaultAsync(p => p.Id == playerId, ct);

            if (player == null)
                return OperationResult.Error($"Игрок с ID '{playerId}' не найден");

            _db.Players.Remove(player);
            await _db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        }
        catch (OperationCanceledException)
        {
            return OperationResult.Error("Операция была отменена");
        }
        catch (DbUpdateException ex)
        {
            return OperationResult.Error($"Ошибка при удалении игрока: {ex.Message}");
        }
        catch (Exception ex)
        {
            return OperationResult.Error($"Неожиданная ошибка: {ex.Message}");
        }
    }
}