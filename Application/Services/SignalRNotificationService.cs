using Application.Notifications;
using Application.Result;
using Microsoft.AspNetCore.SignalR;
using AppHub = Application.Endpoints.Hubs.AppHub;

namespace Application.Services;

public class SignalRNotificationService(IHubContext<AppHub> hubContext) : INotificationService
{
    public async Task<OperationResult> NotifyGameRoomAsync<T>(Guid roomId, T notification) where T : GameNotification
    {
        var groupName = $"room-{roomId}";
        await hubContext.Clients.Group(groupName).SendAsync(notification.MethodName, notification);
        return OperationResult.Ok();
    }
}