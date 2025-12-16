using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Application.Endpoints.Hubs;

public partial class AppHub(IGameManager gameManager, IRoomManager roomManager, IConnectionService connectionService) 
    : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var getPlayerResult = await connectionService.GetPlayerByConnection(Context.ConnectionId);
        if (getPlayerResult.Success)
        {
            var (playerId, roomId) = getPlayerResult.ResultObj;
            await roomManager.LeaveRoomAsync(roomId, playerId);
            await connectionService.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{roomId}");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}