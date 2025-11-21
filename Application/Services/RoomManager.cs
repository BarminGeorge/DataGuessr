using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Result;
using Domain.Entities;

namespace Application.Services;

public class RoomManager(
    IRoomRepository roomRepository, 
    ILogger<RoomManager> logger, 
    IPlayerRepository playerRepository) : IRoomManager
{
    public async Task<ServiceResult<Room>> CreateRoomAsync(Guid userId, RoomPrivacy privacy, string? password = null, int maxPlayers = 4)
    {
        var room = new Room(userId);

        await roomRepository.AddAsync(room);
        logger.LogInformation($"Room {room.Id} created by user {userId}");
        
        return ServiceResult<Room>.Ok(room);
    }

    public async Task<ServiceResult<Room>> JoinRoomAsync(Guid roomId, Guid userId, string? password = null)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null)
            return ServiceResult<Room>.Error($"Room {roomId} not found");
        
        var player = await playerRepository.GetPlayerByIdAsync(userId);
        room.AddPlayer(player);
        await roomRepository.UpdateAsync(room);
        
        logger.LogInformation($"User {userId} joined room {roomId}");
        return ServiceResult<Room>.Ok(room);
    }

    public async Task<ServiceResult> LeaveRoomAsync(Guid roomId, Guid userId)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null) 
            return ServiceResult.Error("Cant room leave, room not found");
        
        var player = await playerRepository.GetPlayerByIdAsync(userId);
        room.RemovePlayer(player);
        
        logger.LogInformation($"User {userId} left room {roomId}");
        return ServiceResult.Ok();
    }
    
    public async Task<ServiceResult<Room>> FindOrCreateQuickRoomAsync(Guid userId)
    {
        var availableRoom = await roomRepository.GetWaitingPublicRoomsAsync();
        
        if (availableRoom == null)
        {
            availableRoom = [(await CreateRoomAsync(userId, RoomPrivacy.Public)).ResultObj];
        }
        
        foreach (var room in availableRoom)
        {
            var result = await JoinRoomAsync(room.Id, userId);
            if (result == null)
                break;
            logger.LogInformation($"No suitable rooms found, creating new quick match for user {userId}");
            return ServiceResult<Room>.Ok(room);
        }
        
        return ServiceResult<Room>.Error("No suitable rooms found");
    }

    public async Task<IEnumerable<Room>?> GetAvailablePublicRoomsAsync()
        => await roomRepository.GetWaitingPublicRoomsAsync();

    // TODO: нужно реализовать связь между соединением, id пользователя и комнаты. Выходим из команты при ошибке
    public Task<ServiceResult> HandleUserError(string connectionId)
    {
        throw new NotImplementedException();
    }

    public async Task<Room?> GetRoomByIdAsync(Guid roomId) 
        => await roomRepository.GetByIdAsync(roomId);
}