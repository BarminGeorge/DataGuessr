using Application.Interfaces;
using Application.Notifications;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;

namespace Application.Services;

public class RoomManager(
    IRoomRepository roomRepository, 
    ILogger<RoomManager> logger, 
    INotificationService notificationService,
    IPlayerRepository playerRepository,
    IUserRepository usersRepository) : IRoomManager
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

        var playerNameResult = await usersRepository.GetPlayerNameByIdAsync(userId, ct);
        if (!playerNameResult.Success)
            return OperationResult<Room>.Error(playerNameResult.ErrorMsg);
        
        var notification = new NewPlayerNotification(player.Id, playerNameResult.ResultObj);
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
        var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));
        if (!notifyResult.Success)
            return OperationResult<Room>.Error(notifyResult.ErrorMsg);
        
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
            var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
            var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));
            if (!notifyResult.Success)
                return OperationResult.Error(notifyResult.ErrorMsg);
            
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