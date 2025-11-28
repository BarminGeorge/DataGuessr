using Domain.Entities;
using Domain.Common;
using Domain.ValueTypes;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Infrastructure;

namespace Infrastructure.PostgreSQL.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext db;

    public PlayerRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<Player>> GetPlayerByIdAsync(Guid playerId, CancellationToken ct = default)
    {
        try
        {
            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == playerId, ct);

            return player != null
                ? OperationResult<Player>.Ok(player)
                : OperationResult<Player>.Error($"Игрок с ID '{playerId}' не найден");
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

    public async Task<OperationResult<Player>> CreatePlayerAsync(Guid userId, Guid roomId, CancellationToken ct = default)
    {
        try
        {
            // Проверяем существует ли пользователь
            var userExists = await db.Users.AnyAsync(u => u.Id == userId, ct);
            if (!userExists)
                return OperationResult<Player>.Error($"Пользователь с ID '{userId}' не найден");

            // Проверяем комнату
            var roomExists = await db.Rooms.AnyAsync(r => r.Id == roomId, ct);
            if (!roomExists)
                return OperationResult<Player>.Error($"Комната с ID '{roomId}' не найдена");

            // Проверяем нет ли уже игрока
            var playerExists = await db.Players.AnyAsync(p => p.UserId == userId && p.RoomId == roomId, ct);
            if (playerExists)
                return OperationResult<Player>.Error("Игрок для этого пользователя уже существует в данной комнате");

            // Создаём Player
            var player = new Player(userId, roomId);

            await db.Players.AddAsync(player, ct);
            await db.SaveChangesAsync(ct);

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

    public async Task<OperationResult> DeletePlayerAsync(Guid playerId, CancellationToken ct = default)
    {
        try
        {
            var player = await db.Players
                .FirstOrDefaultAsync(p => p.Id == playerId, ct);

            if (player == null)
                return OperationResult.Error($"Игрок с ID '{playerId}' не найден");

            db.Players.Remove(player);
            await db.SaveChangesAsync(ct);

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

    public async Task<OperationResult<Player>> GetPlayerByUserIdAndRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default)
    {
        try
        {
            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.RoomId == roomId, ct);

            return player != null
                ? OperationResult<Player>.Ok(player)
                : OperationResult<Player>.Error($"Игрок для пользователя '{userId}' в комнате '{roomId}' не найден");
        }
        catch (OperationCanceledException)
        {
            return OperationResult<Player>.Error("Операция была отменена");
        }
        catch (Exception ex)
        {
            return OperationResult<Player>.Error($"Ошибка при поиске игрока: {ex.Message}");
        }
    }

    public async Task<OperationResult<List<Player>>> GetPlayersByRoomAsync(Guid roomId, CancellationToken ct = default)
    {
        try
        {
            var players = await db.Players
                .AsNoTracking()
                .Where(p => p.RoomId == roomId)
                .ToListAsync(ct);

            return OperationResult<List<Player>>.Ok(players);
        }
        catch (OperationCanceledException)
        {
            return OperationResult<List<Player>>.Error("Операция была отменена");
        }
        catch (Exception ex)
        {
            return OperationResult<List<Player>>.Error($"Ошибка при получении игроков комнаты: {ex.Message}");
        }
    }
}