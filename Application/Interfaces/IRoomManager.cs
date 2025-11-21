using Application.Result;
using Domain.Entities;

namespace Application.Interfaces;

public interface IRoomManager
{
    Task<ServiceResult<Room>> CreateRoomAsync(Guid userId, RoomPrivacy privacy, string? password = null, int maxPlayers = 4);
    Task<ServiceResult<Room>> JoinRoomAsync(Guid roomId, Guid userId, string? password = null);
    Task<ServiceResult> LeaveRoomAsync(Guid roomId, Guid userId);
    Task<IEnumerable<Room>?> GetAvailablePublicRoomsAsync();
    Task<ServiceResult<Room>> FindOrCreateQuickRoomAsync(Guid userId);
    Task<ServiceResult> HandleUserError(string connectionId);
}

public enum RoomPrivacy
{
    Public,
    Private
}