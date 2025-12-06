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
            return addRoomResult.ConvertToOperationResult<Room>();
        
        logger.LogInformation($"Room {room.Id} created by user {userId}");
        
        return OperationResult<Room>.Ok(room);
    }

    public async Task<OperationResult<Room>> JoinRoomAsync(Guid roomId, Guid userId, CancellationToken ct, string? password = null)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success || getRoomResult.ResultObj == null)
            return getRoomResult;

        var room = getRoomResult.ResultObj;
        if (room.Privacy == RoomPrivacy.Private && password != room.Password)
            return OperationResult<Room>.Error.Unauthorized();
            
        var getPlayerResult = await playerRepository.GetPlayerByIdAsync(userId, ct);
        if (!getPlayerResult.Success || getPlayerResult.ResultObj == null)
            return getPlayerResult.ConvertToOperationResult<Room>();
        
        var player = getPlayerResult.ResultObj;
        room.AddPlayer(player);

        var playerNameResult = await usersRepository.GetPlayerNameByIdAsync(userId, ct);
        if (!playerNameResult.Success || playerNameResult.ResultObj == null)
            return playerNameResult.ConvertToOperationResult<Room>();
        
        var notification = new NewPlayerNotification(player.Id, playerNameResult.ResultObj);
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
        var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));
        if (!notifyResult.Success)
            return notifyResult.ConvertToOperationResult<Room>();
        
        var updateResult = await roomRepository.UpdateAsync(room, ct);
        if (!updateResult.Success)
            return updateResult.ConvertToOperationResult<Room>();
        
        logger.LogInformation($"User {userId} joined room {roomId}");
        return OperationResult<Room>.Ok(room);
    }

    public async Task<OperationResult> LeaveRoomAsync(Guid roomId, Guid userId, CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success || getRoomResult.ResultObj == null) 
            return getRoomResult;
        
        var room = getRoomResult.ResultObj;
        
        var getPlayerResult = await playerRepository.GetPlayerByIdAsync(userId, ct);
        if (!getPlayerResult.Success  || getPlayerResult.ResultObj == null)
            return getPlayerResult;
        
        var player = getPlayerResult.ResultObj;
        
        room.RemovePlayer(player);
        
        var notification = new PlayerLeavedNotification(userId, room.Owner);
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
        var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));
        if (!notifyResult.Success)
            return notifyResult;
            
        return await roomRepository.UpdateAsync(room, ct);
    }
    
    public async Task<OperationResult<Room>> FindOrCreateQuickRoomAsync(Guid userId, CancellationToken ct)
    {
        var availableRoomResult = await roomRepository.GetWaitingPublicRoomsAsync(ct);
        if (!availableRoomResult.Success || availableRoomResult.ResultObj == null)
            return await CreateRoomAsync(userId, RoomPrivacy.Public, ct);
        
        var rooms =  availableRoomResult.ResultObj;
        foreach (var room in rooms)
        {
            var joinRoomResult = await JoinRoomAsync(room.Id, userId, ct);
            if (!joinRoomResult.Success)
                continue;
            
            return OperationResult<Room>.Ok(room);
        }

        return OperationResult<Room>.Error.InternalError("Cannot create or find a room");
    }

    public async Task<OperationResult<IEnumerable<Room>>> GetAvailablePublicRoomsAsync(CancellationToken ct)
    {
        return await roomRepository.GetWaitingPublicRoomsAsync(ct);
    }

    public async Task<OperationResult<RoomPrivacy>> GetRoomPrivacyAsync(Guid roomId, CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success || getRoomResult.ResultObj == null)
            return getRoomResult.ConvertToOperationResult<RoomPrivacy>();

        return OperationResult<RoomPrivacy>.Ok(getRoomResult.ResultObj.Privacy);
    }

    public async Task<OperationResult> KickPlayerFromRoom(Guid userId, Guid roomId, Guid removedPlayer, CancellationToken ct)
    {
        var roomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!roomResult.Success || roomResult.ResultObj is null)
            return roomResult;

        if (userId != roomResult.ResultObj.Owner)
            return OperationResult.Error.InvalidOperation("Can't kick player from room, you are not a owner");
        
        return await LeaveRoomAsync(roomId, removedPlayer, ct);
    }
}