using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Interfaces;

namespace Application.Services;

public class GuestCleanupService(IDataContext context) : IGuestCleanupService
{
    public async Task<OperationResult> CleanupOrphanedGuestsAsync(CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            var orphanedGuests = await context.Users
                .Where(u => u.IsGuest)
                .Where(u => !context.Players.Any(p => p.UserId == u.Id))
                .Include(user => user.Avatar)
                .ToListAsync(ct);

            if (orphanedGuests.Count == 0)
                return OperationResult.Ok();

            foreach (var guest in orphanedGuests)
            {
                if (guest.Avatar != null)
                {
                    context.Avatars.Remove(guest.Avatar);
                }
                context.Users.Remove(guest);
            }

            await context.SaveChangesAsync(ct);
            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, TimeSpan.FromMilliseconds(100));
    }

    public async Task<OperationResult> CleanupExpiredRoomsAsync(CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            var expiredRooms = await context.Rooms
                .Where(r => r.ClosedAt < DateTime.UtcNow)
                .ToListAsync(ct);

            if (expiredRooms.Count == 0)
                return OperationResult.Ok();

            context.Rooms.RemoveRange(expiredRooms);
            await context.SaveChangesAsync(ct);

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, TimeSpan.FromMilliseconds(100));
    }
}
