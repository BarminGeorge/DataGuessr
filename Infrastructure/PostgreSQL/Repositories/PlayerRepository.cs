using Domain.Entities;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Interfaces;

namespace Infrastructure.PostgreSQL.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext db;

    public PlayerRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<Player>> GetPlayerByIdAsync(Guid playerId, CancellationToken ct)
    {
        return await OperationResult<Player>.TryAsync(async () =>
        {
            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == playerId, ct)
                ?? throw new KeyNotFoundException($"Игрок с ID '{playerId}' не найден");

            return player;
        });
    }

    public async Task<OperationResult<Player>> GetPlayerByUserIdAndRoomAsync(
        Guid userId,
        Guid roomId,
        CancellationToken ct = default)
    {
        return await OperationResult<Player>.TryAsync(async () =>
        {
            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.RoomId == roomId, ct)
                ?? throw new KeyNotFoundException($"Игрок для пользователя '{userId}' в комнате '{roomId}' не найден");

            return player;
        });
    }

    public async Task<OperationResult<List<Player>>> GetPlayersByRoomAsync(
        Guid roomId,
        CancellationToken ct = default)
    {
        return await OperationResult<List<Player>>.TryAsync(async () =>
        {
            var players = await db.Players
                .AsNoTracking()
                .Where(p => p.RoomId == roomId)
                .ToListAsync(ct);

            return players;
        });
    }

    public async Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnectionIdAsync(string connectionId)
    {
        return await OperationResult<(Guid, Guid)>.TryAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                throw new ArgumentException("ConnectionId не может быть пустым");

            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId)
                ?? throw new KeyNotFoundException($"Игрок с ConnectionId '{connectionId}' не найден");

            return (player.Id, player.RoomId);
        });
    }

    public async Task<OperationResult> CreatePlayerAsync(
        string connectionId,
        Guid userId,
        Guid roomId,
        CancellationToken ct)
    {
        return await OperationResult.TryAsync(async () =>
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
        });
    }

    public async Task<OperationResult> DeletePlayerAsync(Guid playerId, CancellationToken ct = default)
    {
        return await OperationResult.TryAsync(async () =>
        {
            var player = await db.Players
                .FirstOrDefaultAsync(p => p.Id == playerId, ct)
                ?? throw new KeyNotFoundException($"Игрок с ID '{playerId}' не найден");

            db.Players.Remove(player);
            await db.SaveChangesAsync(ct);
        });
    }

    public async Task<OperationResult> RemovePlayerByConnectionAsync(string connectionId)
    {
        return await OperationResult.TryAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                throw new ArgumentException("ConnectionId не может быть пустым");

            var player = await db.Players
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId)
                ?? throw new KeyNotFoundException($"Игрок с ConnectionId '{connectionId}' не найден");

            db.Players.Remove(player);
            await db.SaveChangesAsync();
        });
    }
}
