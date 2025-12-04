using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public RoomRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<Room>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        Func<Task<OperationResult<Room>>> operation = async () =>
        {
            var room = await db.Rooms
                .AsNoTracking()
                .Include(r => r.Players)
                .Include(r => r.Games)
                .FirstOrDefaultAsync(r => r.Id == id, ct)
                ?? throw new KeyNotFoundException($"Комната с ID '{id}' не найдена");

            return OperationResult<Room>.Ok(room);
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult<IEnumerable<Room>>> GetWaitingPublicRoomsAsync(CancellationToken ct)
    {
        Func<Task<OperationResult<IEnumerable<Room>>>> operation = async () =>
        {
            var rooms = await db.Rooms
                .AsNoTracking()
                .Where(r => r.Privacy == RoomPrivacy.Public
                    && r.Status == RoomStatus.Available
                    && r.ClosedAt > DateTime.UtcNow)
                .Include(r => r.Players)
                .ToListAsync(ct);

            return OperationResult<IEnumerable<Room>>.Ok(rooms);
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult> AddAsync(Room room, CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            await db.Rooms.AddAsync(room, ct);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult> UpdateAsync(Room room, CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            var existingRoom = await db.Rooms.AnyAsync(r => r.Id == room.Id, ct);
            if (!existingRoom)
                throw new KeyNotFoundException($"Комната с ID '{room.Id}' не найдена");

            db.Rooms.Update(room);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult> RemoveAsync(Guid roomId, CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            var room = await db.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId, ct)
                ?? throw new KeyNotFoundException($"Комната с ID '{roomId}' не найдена");

            db.Rooms.Remove(room);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult<Game>> GetCurrentGameAsync(Guid roomId, CancellationToken ct)
    {
        Func<Task<OperationResult<Game>>> operation = async () =>
        {
            var roomExists = await db.Rooms.AnyAsync(r => r.Id == roomId, ct);
            if (!roomExists)
                throw new KeyNotFoundException($"Комната с ID '{roomId}' не найдена");

            var game = await db.Games
                .Where(g => g.RoomId == roomId && g.Status != GameStatus.Finished)
                .Include(g => g.Questions)
                .FirstOrDefaultAsync(ct)
                ?? throw new KeyNotFoundException("Активная игра в комнате не найдена");

            return OperationResult<Game>.Ok(game);
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }
}
