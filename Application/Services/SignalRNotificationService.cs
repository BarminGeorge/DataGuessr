using Application.Hubs;
using Application.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services;

public class SignalRNotificationService(IHubContext<AppHub> hubContext) : INotificationService
{
    public async Task NotifyGameRoomAsync<T>(Guid roomId, T notification) where T : GameNotification
    {
        var groupName = $"room-{roomId}";
        await hubContext.Clients.Group(groupName).SendAsync(notification.MethodName, notification);
    }
}