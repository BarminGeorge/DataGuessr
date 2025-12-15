using Application.Interfaces;
using Application.Mappers;
using Application.Notifications;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;

namespace Application.Services;

public class RoomManager(
    IRoomRepository roomRepository, 
    INotificationService notificationService,
    IPlayerRepository playerRepository,
    IUserRepository usersRepository) : IRoomManager
{
    public async Task<OperationResult<Room>> CreateRoomAsync(Guid userId, RoomPrivacy privacy, CancellationToken ct,
        string? password = null, int maxPlayers = 15)
    {
        var room = new Room(userId, privacy, maxPlayers, password);
        Console.WriteLine($"20 {room.Players.Count}");
        var addRoomResult = await roomRepository.AddAsync(room, ct);
        if (!addRoomResult.Success)
            return addRoomResult.ConvertToOperationResult<Room>();
        Console.WriteLine($"24 {room.Players.Count}");
        return OperationResult<Room>.Ok(room);
    }

    public async Task<OperationResult<Room>> JoinRoomAsync(Guid roomId, Guid userId, CancellationToken ct, string? password = null)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success || getRoomResult.ResultObj == null)
            return getRoomResult;
        Console.WriteLine($"33 {getRoomResult.ResultObj.Players.Count}");
        var room = getRoomResult.ResultObj;
        if (room.Privacy == RoomPrivacy.Private && password != room.Password)
            return OperationResult<Room>.Error.Unauthorized();
            
        var getPlayerResult = await playerRepository.GetPlayerByUserIdAsync(userId, ct);
        if (!getPlayerResult.Success || getPlayerResult.ResultObj == null)
            return getPlayerResult.ConvertToOperationResult<Room>();
        
        var player = getPlayerResult.ResultObj;
        Console.WriteLine($"43 {room.Players.Count}");
        room.AddPlayer(player);
        Console.WriteLine($"46 {room.Players.Count}");
        var getUsersResult = await usersRepository.GetUsersByIds([userId], ct);
        if (!getUsersResult.Success || getUsersResult.ResultObj == null)
            return getUsersResult.ConvertToOperationResult<Room>();
        player.SetUserInfo(getUsersResult.ResultObj.First());
        Console.WriteLine($"50 {room.Players.Count}");
        var notification = new NewPlayerNotification(player.ToDto());
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
        var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));
        if (!notifyResult.Success)
            return notifyResult.ConvertToOperationResult<Room>();
        Console.WriteLine($"57 {room.Players.Count}");
        var updateResult = await roomRepository.UpdateAsync(room, ct);
        if (!updateResult.Success)
            return updateResult.ConvertToOperationResult<Room>();
        Console.WriteLine($"60 {room.Players.Count}");
        var usersResult = await usersRepository.GetUsersByIds(room.Players.Select(x => x.UserId), ct);
        if (!usersResult.Success || usersResult.ResultObj == null)
            return usersResult.ConvertToOperationResult<Room>();
        room.FillPlayersWithUserInfo(usersResult.ResultObj);
        Console.WriteLine($"65 {room.Players.Count}");
        return OperationResult<Room>.Ok(room);
    }

    public async Task<OperationResult> LeaveRoomAsync(Guid roomId, Guid userId, CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success || getRoomResult.ResultObj == null) 
            return getRoomResult;
        
        var room = getRoomResult.ResultObj;
        
        var getPlayerResult = await playerRepository.GetPlayerByUserIdAsync(userId, ct);
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