using Domain.Common;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.PostgreSQL;
using Application.Interfaces;

namespace Application.Services;

public class GuestCleanupService : IGuestCleanupService
{
    private readonly AppDbContext _context;

    public GuestCleanupService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult> CleanupOrphanedGuestsAsync(CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            var orphanedGuests = await _context.Users
                .Where(u => u.IsGuest)
                .Where(u => !_context.Players.Any(p => p.UserId == u.Id))
                .ToListAsync(ct);

            if (orphanedGuests.Count == 0)
                return OperationResult.Ok();

            foreach (var guest in orphanedGuests)
            {
                if (guest.Avatar != null)
                {
                    _context.Avatars.Remove(guest.Avatar);
                }
                _context.Users.Remove(guest);
            }

            await _context.SaveChangesAsync(ct);
            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, TimeSpan.FromMilliseconds(100));
    }

    public async Task<OperationResult> CleanupExpiredRoomsAsync(CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            var expiredRooms = await _context.Rooms
                .Where(r => r.ClosedAt < DateTime.UtcNow)
                .ToListAsync(ct);

            if (expiredRooms.Count == 0)
                return OperationResult.Ok();

            _context.Rooms.RemoveRange(expiredRooms);
            await _context.SaveChangesAsync(ct);

            // Триггер автоматически удалит orphaned гостей
            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, TimeSpan.FromMilliseconds(100));
    }
}
