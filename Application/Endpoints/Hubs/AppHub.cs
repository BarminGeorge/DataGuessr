using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Application.Endpoints.Hubs;

public partial class AppHub(IGameManager gameManager, IRoomManager roomManager) 
    : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await roomManager.HandleUserError(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}