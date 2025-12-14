using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly IDataContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public RoomRepository(IDataContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult<Room>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<Room>>>(async () =>
        {
            if (id == Guid.Empty)
                return OperationResult<Room>.Error.Validation("Id не может быть пустым GUID");

            var room = await db.Rooms
            .Include(r => r.Players)
            .Include(r => r.Games)     
            .AsNoTracking()              
            .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (room == null)
                return OperationResult<Room>.Error.NotFound($"Комната с ID '{id}' не найдена");

            return OperationResult<Room>.Ok(room);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }


    public async Task<OperationResult<Room>> GetByIdAsyncForTesting(Guid id, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<Room>>>(async () =>
        {
            if (id == Guid.Empty)
                return OperationResult<Room>.Error.Validation("Id не может быть пустым GUID");

            var room = await db.Rooms
                .Include(r => r.Players)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (room == null)
                return OperationResult<Room>.Error.NotFound($"Комната с ID '{id}' не найдена");

            return OperationResult<Room>.Ok(room);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }

    public async Task<OperationResult<IEnumerable<Room>>> GetWaitingPublicRoomsAsync(CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<IEnumerable<Room>>>>(async () =>
        {
            var rooms = await db.Rooms
                .AsNoTracking()
                .Where(r => r.Privacy == RoomPrivacy.Public
                    && r.Status == RoomStatus.Available
                    && r.ClosedAt > DateTime.UtcNow)
                .Include(r => r.Players)
                .ToListAsync(ct);

            return OperationResult<IEnumerable<Room>>.Ok(rooms);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }

    public async Task<OperationResult> AddAsync(Room room, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (room == null)
                return OperationResult.Error.Validation("Комната не может быть null");

            await db.Rooms.AddAsync(room, ct);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }

    public async Task<OperationResult> UpdateAsync(Room room, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (room == null)
                return OperationResult.Error.Validation("Комната не может быть null");

            if (room.Id == Guid.Empty)
                return OperationResult.Error.Validation("Id комнаты не может быть пустым GUID");

            var existingRoom = await db.Rooms.AnyAsync(r => r.Id == room.Id, ct);
            if (!existingRoom)
                return OperationResult.Error.NotFound($"Комната с ID '{room.Id}' не найдена");

            db.Rooms.Update(room);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }


    public async Task<OperationResult<Game>> GetCurrentGameAsync(Guid roomId, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<Game>>>(async () =>
        {
            if (roomId == Guid.Empty)
                return OperationResult<Game>.Error.Validation("RoomId не может быть пустым GUID");

            var roomExists = await db.Rooms.AnyAsync(r => r.Id == roomId, ct);
            if (!roomExists)
                return OperationResult<Game>.Error.NotFound($"Комната с ID '{roomId}' не найдена");

            var game = await db.Games
                .Where(g => g.RoomId == roomId && g.Status != GameStatus.Finished)
                .Include(g => g.Questions)
                .FirstOrDefaultAsync(ct);

            if (game == null)
                return OperationResult<Game>.Error.NotFound("Активная игра в комнате не найдена");

            return OperationResult<Game>.Ok(game);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }
}