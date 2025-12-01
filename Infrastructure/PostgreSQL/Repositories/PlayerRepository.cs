using Domain.Entities;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Interfaces;

namespace Infrastructure.PostgreSQL.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public PlayerRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<Player>> GetPlayerByIdAsync(Guid playerId, CancellationToken ct)
    {
        Func<Task<OperationResult<Player>>> operation = async () =>
        {
            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == playerId, ct)
                ?? throw new KeyNotFoundException($"Игрок с ID '{playerId}' не найден");

            return OperationResult<Player>.Ok(player);
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult<Player>> GetPlayerByUserIdAndRoomAsync(
        Guid userId,
        Guid roomId,
        CancellationToken ct = default)
    {
        Func<Task<OperationResult<Player>>> operation = async () =>
        {
            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.RoomId == roomId, ct)
                ?? throw new KeyNotFoundException($"Игрок для пользователя '{userId}' в комнате '{roomId}' не найден");

            return OperationResult<Player>.Ok(player);
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult<List<Player>>> GetPlayersByRoomAsync(
        Guid roomId,
        CancellationToken ct = default)
    {
        Func<Task<OperationResult<List<Player>>>> operation = async () =>
        {
            var players = await db.Players
                .AsNoTracking()
                .Where(p => p.RoomId == roomId)
                .ToListAsync(ct);

            return OperationResult<List<Player>>.Ok(players);
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnectionIdAsync(string connectionId)
    {
        Func<Task<OperationResult<(Guid, Guid)>>> operation = async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                throw new ArgumentException("ConnectionId не может быть пустым");

            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId)
                ?? throw new KeyNotFoundException($"Игрок с ConnectionId '{connectionId}' не найден");

            return OperationResult<(Guid, Guid)>.Ok((player.Id, player.RoomId));
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult> CreatePlayerAsync(
        string connectionId,
        Guid userId,
        Guid roomId,
        CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                throw new ArgumentException("ConnectionId не может быть пустым");

            var room = await db.Rooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Id == roomId, ct)
                ?? throw new KeyNotFoundException($"Комната с ID '{roomId}' не найдена");

            if (room.IsExpired)
                throw new InvalidOperationException("Комната истекла");

            if (room.Players.Count >= room.MaxPlayers)
                throw new InvalidOperationException("Комната заполнена");

            if (room.Players.Any(p => p.UserId == userId))
                throw new InvalidOperationException("Пользователь уже в этой комнате");

            var userExists = await db.Users.AnyAsync(u => u.Id == userId, ct);
            if (!userExists)
                throw new KeyNotFoundException($"Пользователь с ID '{userId}' не найден");

            var connectionExists = await db.Players.AnyAsync(p => p.ConnectionId == connectionId, ct);
            if (connectionExists)
                throw new InvalidOperationException($"ConnectionId '{connectionId}' уже занят");

            var player = new Player(userId, roomId, connectionId);
            await db.Players.AddAsync(player, ct);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult> DeletePlayerAsync(Guid playerId, CancellationToken ct = default)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            var player = await db.Players
                .FirstOrDefaultAsync(p => p.Id == playerId, ct)
                ?? throw new KeyNotFoundException($"Игрок с ID '{playerId}' не найден");

            db.Players.Remove(player);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult> RemovePlayerByConnectionAsync(string connectionId)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                throw new ArgumentException("ConnectionId не может быть пустым");

            var player = await db.Players
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId)
                ?? throw new KeyNotFoundException($"Игрок с ConnectionId '{connectionId}' не найден");

            db.Players.Remove(player);
            await db.SaveChangesAsync();

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }
}
