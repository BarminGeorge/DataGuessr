using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Result;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class RoomManager(
    IRoomRepository roomRepository, 
    ILogger<RoomManager> logger, 
    IPlayerRepository playerRepository) : IRoomManager
{
    public async Task<OperationResult<Room>> CreateRoomAsync(Guid userId, RoomPrivacy privacy, string? password = null, int maxPlayers = 4)
    {
        var room = new Room(userId, privacy, maxPlayers, password);

        var addRoomResult = await roomRepository.AddAsync(room);
        if (!addRoomResult.Success)
            return OperationResult<Room>.Error(addRoomResult.ErrorMsg);
        
        logger.LogInformation($"Room {room.Id} created by user {userId}");
        
        return OperationResult<Room>.Ok(room);
    }

    public async Task<OperationResult<Room>> JoinRoomAsync(Guid roomId, Guid userId, string? password = null)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId);
        if (!getRoomResult.Success)
            return OperationResult<Room>.Error(getRoomResult.ErrorMsg);

        var room = getRoomResult.ResultObj;
        
        var getPlayerResult = await playerRepository.GetPlayerByIdAsync(userId);
        if (!getPlayerResult.Success)
            return OperationResult<Room>.Error(getPlayerResult.ErrorMsg);
        
        var player = getPlayerResult.ResultObj;
        room.AddPlayer(player);
        
        var updateResult = await roomRepository.UpdateAsync(room);
        if (!updateResult.Success)
            return OperationResult<Room>.Error(updateResult.ErrorMsg);
        
        logger.LogInformation($"User {userId} joined room {roomId}");
        return OperationResult<Room>.Ok(room);
    }

    public async Task<OperationResult> LeaveRoomAsync(Guid roomId, Guid userId)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId);
        if (!getRoomResult.Success) 
            return OperationResult.Error(getRoomResult.ErrorMsg);
        
        var room =  getRoomResult.ResultObj;
        
        var getPlayerResult = await playerRepository.GetPlayerByIdAsync(userId);
        if (!getPlayerResult.Success)
            return OperationResult.Error(getPlayerResult.ErrorMsg);
        
        var player = getPlayerResult.ResultObj;
        
        room.RemovePlayer(player);
        logger.LogInformation($"User {userId} left room {roomId}");
        return OperationResult.Ok();
    }
    
    public async Task<OperationResult<Room>> FindOrCreateQuickRoomAsync(Guid userId)
    {
        var availableRoomResult = await roomRepository.GetWaitingPublicRoomsAsync();
        var rooms =  availableRoomResult.ResultObj;
        if (!availableRoomResult.Success)
        {
            var creatingResult = await CreateRoomAsync(userId, RoomPrivacy.Public);
            if (!creatingResult.Success)
                return OperationResult<Room>.Error(creatingResult.ErrorMsg);
            
            rooms = [creatingResult.ResultObj];
        }
        
        foreach (var room in rooms)
        {
            var joinRoomResult = await JoinRoomAsync(room.Id, userId);
            if (!joinRoomResult.Success)
                break;
            logger.LogInformation($"No suitable rooms found, creating new quick match for user {userId}");
            return OperationResult<Room>.Ok(room);
        }
        
        return OperationResult<Room>.Error("No suitable rooms found");
    }

    public async Task<OperationResult<IEnumerable<Room>>> GetAvailablePublicRoomsAsync()
    {
        return await roomRepository.GetWaitingPublicRoomsAsync();
    }
}