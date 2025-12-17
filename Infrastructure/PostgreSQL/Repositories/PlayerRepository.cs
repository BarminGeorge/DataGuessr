using Domain.Entities;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Interfaces;

namespace Infrastructure.PostgreSQL.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly IDataContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public PlayerRepository(IDataContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<Player>> GetPlayerByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<Player>>>(async () =>
        {
            if (userId == Guid.Empty)
                return OperationResult<Player>.Error.Validation("UserId не может быть пустым GUID");
            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId, ct);

            if (player == null)
                return OperationResult<Player>.Error.NotFound($"Игрок с UserId '{userId}' не найден");

            return OperationResult<Player>.Ok(player);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }


    public async Task<OperationResult<(Guid playerId, Guid roomId)>> GetPlayerByConnectionIdAsync(string connectionId)
    {
        var operation = new Func<Task<OperationResult<(Guid, Guid)>>>(async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                return OperationResult<(Guid, Guid)>.Error.Validation("ConnectionId не может быть пустым");

            var player = await db.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId);

            if (player == null)
                return OperationResult<(Guid, Guid)>.Error.NotFound($"Игрок с ConnectionId '{connectionId}' не найден");

            return OperationResult<(Guid, Guid)>.Ok((player.Id, player.RoomId));
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }

    public async Task<OperationResult> CreatePlayerAsync(
        string connectionId,
        Guid userId,
        Guid roomId,
        CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                return OperationResult.Error.Validation("ConnectionId не может быть пустым");

            if (userId == Guid.Empty)
                return OperationResult.Error.Validation("UserId не может быть пустым GUID");

            if (roomId == Guid.Empty)
                return OperationResult.Error.Validation("RoomId не может быть пустым GUID");

            var room = await db.Rooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Id == roomId, ct);

            if (room == null)
                return OperationResult.Error.NotFound($"Комната с ID '{roomId}' не найдена");

            if (room.IsExpired)
                return OperationResult.Error.InvalidOperation("Комната истекла");

            if (room.Players.Count >= room.MaxPlayers)
                return OperationResult.Error.InvalidOperation("Комната заполнена");

            if (room.Players.Any(p => p.UserId == userId))
                return OperationResult.Error.AlreadyExists("Пользователь уже в этой комнате");

            var userExists = await db.Users.AnyAsync(u => u.Id == userId, ct);
            if (!userExists)
                return OperationResult.Error.NotFound($"Пользователь с ID '{userId}' не найден");

            var connectionExists = await db.Players.AnyAsync(p => p.ConnectionId == connectionId, ct);
            if (connectionExists)
                return OperationResult.Error.AlreadyExists($"ConnectionId '{connectionId}' уже занят");

            var player = new Player(userId, roomId, connectionId);
            await db.Players.AddAsync(player, ct);
            await db.SaveChangesAsync(ct);
            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }


    public async Task<OperationResult> RemovePlayerByConnectionAsync(string connectionId)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                return OperationResult.Error.Validation("ConnectionId не может быть пустым");

            var player = await db.Players
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId);

            if (player == null)
                return OperationResult.Error.NotFound($"Игрок с ConnectionId '{connectionId}' не найден");

            db.Players.Remove(player);
            await db.SaveChangesAsync();

            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }

    public async Task<OperationResult<string>> GetConnectionByPlayerAsync(Guid playerId, CancellationToken ct = default)
    {
        var operation = new Func<Task<OperationResult<string>>>(async () =>
        {
            if (playerId == Guid.Empty)
                return OperationResult<string>.Error.Validation("PlayerId не может быть пустым GUID");

            var connectionId = await db.Players
                .AsNoTracking()
                .Where(p => p.Id == playerId)
                .Select(p => p.ConnectionId)
                .FirstOrDefaultAsync(ct);

            if (string.IsNullOrEmpty(connectionId))
                return OperationResult<string>.Error.NotFound($"Игрок с ID '{playerId}' не найден");

            return OperationResult<string>.Ok(connectionId);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }
}