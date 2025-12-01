using Domain.Common;

namespace Application.Interfaces;

public interface IGuestCleanupService
{
    Task<OperationResult> CleanupOrphanedGuestsAsync(CancellationToken ct);
    Task<OperationResult> CleanupExpiredRoomsAsync(CancellationToken ct);
}