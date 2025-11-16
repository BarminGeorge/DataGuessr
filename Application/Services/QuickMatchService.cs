using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class QuickMatchService(
    IRoomRepository roomRepository,
    IRoomManager roomManager,
    ILogger<QuickMatchService> logger) : IQuickMatchService
{
    public async Task<Room> FindOrCreateQuickMatchAsync(Guid userId)
    {
        var availableRoom = await roomRepository.GetWaitingPublicRoomsAsync();
        
        if (availableRoom == null)
        {
            availableRoom = [await roomManager.CreateRoomAsync(userId, RoomType.Public)];
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