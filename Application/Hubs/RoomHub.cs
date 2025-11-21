using Application.Mappers;
using Application.Requests_Responses;

namespace Application.Hubs;

public partial class AppHub
{
    public async Task<CreateRoomResponse> CreateRoom(CreateRoomRequest request)
    {
        var room = await roomManager.CreateRoomAsync(request.userId, request.privacy, request.password, request.maxPlayers);
        
        var roomDto = room.ToDto();
        var response = new CreateRoomResponse(true, roomDto);
            
        await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{room.Id}");
        
        return response;
    }

    public async Task<JoinRoomResponse> JoinRoom(JoinRoomRequest request)
    {
        var result = await roomManager.JoinRoomAsync(request.userId, request.roomId, request.password);
        var response = new JoinRoomResponse(true, result.ToDto());
        if (response.Success)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{request.roomId}");
        
        return response;
    }

    public async Task<LeaveRoomResponse> LeaveRoom(LeaveRoomRequest request)
    {
        var result = await roomManager.LeaveRoomAsync(request.userId, request.roomId);
        var response = new LeaveRoomResponse(result);
        if (result)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{request.roomId}");
        
        return response;
    }
}