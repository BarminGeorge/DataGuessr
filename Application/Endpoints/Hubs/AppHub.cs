using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Application.Endpoints.Hubs;

public partial class AppHub(IGameManager gameManager, IRoomManager roomManager, IConnectionService connectionService) 
    : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var getUserResult = await connectionService.GetUserByConnection(Context.ConnectionId);
        if (getUserResult.Success)
        {
            var (userId, roomId) = getUserResult.ResultObj;
            await connectionService.RemoveConnection(Context.ConnectionId);
            await roomManager.LeaveRoomAsync(roomId, userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{roomId}");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}