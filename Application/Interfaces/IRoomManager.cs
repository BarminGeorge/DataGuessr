using Application.Result;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces;

public interface IRoomManager
{
    Task<OperationResult<Room>> CreateRoomAsync(Guid userId, RoomPrivacy privacy, CancellationToken ct, string? password = null, int maxPlayers = 15);
    Task<OperationResult<Room>> JoinRoomAsync(Guid roomId, Guid userId, CancellationToken ct, string? password = null);
    Task<OperationResult> LeaveRoomAsync(Guid roomId, Guid userId, CancellationToken ct = default);
    Task<OperationResult<IEnumerable<Room>>> GetAvailablePublicRoomsAsync(CancellationToken ct);
    Task<OperationResult<Room>> FindOrCreateQuickRoomAsync(Guid userId, CancellationToken ct);
}
