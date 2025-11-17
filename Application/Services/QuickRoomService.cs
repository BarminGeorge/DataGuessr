using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class QuickRoomService(
    IRoomRepository roomRepository,
    IRoomManager roomManager,
    ILogger<QuickRoomService> logger) : IQuickRoomService
{
    public async Task<Room> FindOrCreateQuickRoomAsync(Guid userId)
    {
        var availableRoom = await roomRepository.GetWaitingPublicRoomsAsync();
        
        if (availableRoom == null)
        {
            availableRoom = [await roomManager.CreateRoomAsync(userId, RoomPrivacy.Public)];
        }
        
        foreach (var room in availableRoom)
        {
            var joined = await roomManager.JoinRoomAsync(room.Id, userId);
            if (joined)
                break;
            logger.LogInformation($"No suitable rooms found, creating new quick match for user {userId}");
            return room;
        }
        
        throw new Exception("No suitable rooms found");
    }
}