using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class RoomManager(
    IRoomRepository roomRepository, 
    ILogger<RoomManager> logger, 
    IPlayerRepository playerRepository) : IRoomManager
{
    public async Task<Room> CreateRoomAsync(Guid userId, RoomPrivacy privacy, string? password = null, int maxPlayers = 4)
    {
        var room = new Room();

        await roomRepository.AddAsync(room);
        logger.LogInformation($"Room {room.Id} created by user {userId}");
        
        return room;
    }

    public async Task<Room?> JoinRoomAsync(Guid roomId, Guid userId, string? password = null)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null)
        {
            logger.LogWarning($"Room {roomId} not found");
            return room;
        }
        
        var player = await playerRepository.GetPlayerByIdAsync(userId);
        room.AddPlayer(player);
        await roomRepository.UpdateAsync(room);
        
        logger.LogInformation($"User {userId} joined room {roomId}");
        return room;
    }

    public async Task<bool> LeaveRoomAsync(Guid roomId, Guid userId)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null) 
            return false;
        
        var player = await playerRepository.GetPlayerByIdAsync(userId);
        room.RemovePlayer(player);
        
        logger.LogInformation($"User {userId} left room {roomId}");
        return true;
    }

    public async Task<IEnumerable<Room>?> GetAvailablePublicRoomsAsync()
        => await roomRepository.GetWaitingPublicRoomsAsync();

    // TODO: нужно реализовать связь между соединением, id пользователя и комнаты. Выходим из команты при ошибке
    public Task<bool> HandleUserError(string connectionId)
    {
        throw new NotImplementedException();
    }

    public async Task<Room?> GetRoomByIdAsync(Guid roomId) 
        => await roomRepository.GetByIdAsync(roomId);
}