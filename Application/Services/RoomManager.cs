using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Notifications;
using Application.Result;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class RoomManager(
    IRoomRepository roomRepository, 
    ILogger<RoomManager> logger, 
    INotificationService notificationService,
    IPlayerRepository playerRepository) : IRoomManager
{
    public async Task<OperationResult<Room>> CreateRoomAsync(Guid userId, RoomPrivacy privacy, CancellationToken ct,
        string? password = null, int maxPlayers = 15)
    {
        var room = new Room(userId, privacy, maxPlayers, password);

        var addRoomResult = await roomRepository.AddAsync(room, ct);
        if (!addRoomResult.Success)
            return OperationResult<Room>.Error(addRoomResult.ErrorMsg);
        
        logger.LogInformation($"Room {room.Id} created by user {userId}");
        
        return OperationResult<Room>.Ok(room);
    }

    public async Task<OperationResult<Room>> JoinRoomAsync(Guid roomId, Guid userId, CancellationToken ct, string? password = null)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success)
            return OperationResult<Room>.Error(getRoomResult.ErrorMsg);

        var room = getRoomResult.ResultObj;
        
        var getPlayerResult = await playerRepository.GetPlayerByIdAsync(userId, ct);
        if (!getPlayerResult.Success)
            return OperationResult<Room>.Error(getPlayerResult.ErrorMsg);
        
        var player = getPlayerResult.ResultObj;
        room.AddPlayer(player);

        var notification = new NewPlayerNotification(player.Id, player.User.PlayerName);
        await notificationService.NotifyGameRoomAsync(roomId, notification)
            .WithRetry(3, TimeSpan.FromSeconds(0.2));
        
        var updateResult = await roomRepository.UpdateAsync(room, ct);
        if (!updateResult.Success)
            return OperationResult<Room>.Error(updateResult.ErrorMsg);
        
        logger.LogInformation($"User {userId} joined room {roomId}");
        return OperationResult<Room>.Ok(room);
    }

    public async Task<OperationResult> LeaveRoomAsync(Guid roomId, Guid userId, CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success) 
            return OperationResult.Error(getRoomResult.ErrorMsg);
        
        var room = getRoomResult.ResultObj;
        
        var getPlayerResult = await playerRepository.GetPlayerByIdAsync(userId, ct);
        if (!getPlayerResult.Success)
            return OperationResult.Error(getPlayerResult.ErrorMsg);
        
        var player = getPlayerResult.ResultObj;
        
        room.RemovePlayer(player);
        if (room.Players.Count > 0)
        {
            var notification = new PlayerLeavedNotification(userId, room.Owner);
            await notificationService.NotifyGameRoomAsync(roomId, notification)
                .WithRetry(3, TimeSpan.FromSeconds(0.2));
            return await roomRepository.UpdateAsync(room, ct);
        }

        return await roomRepository.RemoveAsync(roomId, ct);
    }
    
    public async Task<OperationResult<Room>> FindOrCreateQuickRoomAsync(Guid userId, CancellationToken ct)
    {
        var availableRoomResult = await roomRepository.GetWaitingPublicRoomsAsync(ct);
        var rooms =  availableRoomResult.ResultObj;
        if (!availableRoomResult.Success)
        {
            var creatingResult = await CreateRoomAsync(userId, RoomPrivacy.Public, ct);
            if (!creatingResult.Success)
                return OperationResult<Room>.Error(creatingResult.ErrorMsg);
            
            rooms = [creatingResult.ResultObj];
        }
        
        foreach (var room in rooms)
        {
            var joinRoomResult = await JoinRoomAsync(room.Id, userId, ct);
            if (!joinRoomResult.Success)
                break;
            logger.LogInformation($"No suitable rooms found, creating new quick match for user {userId}");
            return OperationResult<Room>.Ok(room);
        }
        
        return OperationResult<Room>.Error("No suitable rooms found");
    }

    public async Task<OperationResult<IEnumerable<Room>>> GetAvailablePublicRoomsAsync(CancellationToken ct)
    {
        return await roomRepository.GetWaitingPublicRoomsAsync(ct);
    }
}