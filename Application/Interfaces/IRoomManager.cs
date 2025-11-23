using Application.Result;
using Domain.Entities;

namespace Application.Interfaces;

public interface IRoomManager
{
    Task<OperationResult<Room>> CreateRoomAsync(Guid userId, RoomPrivacy privacy, string? password = null, int maxPlayers = 4);
    Task<OperationResult<Room>> JoinRoomAsync(Guid roomId, Guid userId, string? password = null);
    Task<OperationResult> LeaveRoomAsync(Guid roomId, Guid userId);
    Task<OperationResult<IEnumerable<Room>>> GetAvailablePublicRoomsAsync();
    Task<OperationResult<Room>> FindOrCreateQuickRoomAsync(Guid userId);
}

public enum RoomPrivacy
{
    Public,
    Private
}