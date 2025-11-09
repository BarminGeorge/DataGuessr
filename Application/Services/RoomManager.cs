using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class RoomManager(
    IRoomRepository roomRepository, 
    ILogger<RoomManager> logger, 
    IPlayerRepository playerRepository) : IRoomManager
{
    public async Task<Room> CreateRoomAsync(Guid userId, RoomType type, string? password = null, int maxPlayers = 4)
    {
        var room = new Room();

        await roomRepository.AddAsync(room);
        logger.LogInformation("Room {RoomId} created by user {UserId}", room.Id, userId);
        
        return room;
    }

    public async Task<bool> JoinRoomAsync(Guid roomId, Guid userId, string? password = null)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null)
        {
            logger.LogWarning("Room {RoomId} not found", roomId);
            return false;
        }
        
        var player = await playerRepository.GetPlayerByIdAsync(userId);
        room.AddPlayer(player);
        await roomRepository.UpdateAsync(room);
        
        logger.LogInformation("User {UserId} joined room {RoomId}", userId, roomId);
        return true;
    }

    public async Task<bool> LeaveRoomAsync(Guid roomId, Guid userId)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null) 
            return false;
        
        var player = await playerRepository.GetPlayerByIdAsync(userId);
        room.RemovePlayer(player);
        
        logger.LogInformation("User {UserId} left room {RoomId}", userId, roomId);
        return true;
    }

    public async Task<IEnumerable<Room>> GetAvailablePublicRoomsAsync() 
        => await roomRepository.GetWaitingPublicRoomsAsync();

    public async Task<Room?> GetRoomByIdAsync(Guid roomId) 
        => await roomRepository.GetByIdAsync(roomId);
}