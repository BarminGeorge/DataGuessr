using Domain.Entities;

namespace Application.Interfaces;

public interface IRoomManager
{
    Task<Room> CreateRoomAsync(Guid userId, RoomPrivacy privacy, string? password = null, int maxPlayers = 4);
    Task<bool> JoinRoomAsync(Guid roomId, Guid userId, string? password = null);
    Task<bool> LeaveRoomAsync(Guid roomId, Guid userId);
    Task<IEnumerable<Room>?> GetAvailablePublicRoomsAsync();
    Task<Room?> GetRoomByIdAsync(Guid roomId);
}

public enum RoomPrivacy
{
    Public,
    Private
}