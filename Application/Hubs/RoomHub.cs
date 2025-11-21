using Application.Dto;
using Application.Mappers;
using Application.Requests_Responses;

namespace Application.Hubs;

public partial class AppHub
{
    public async Task<DataResponse<RoomDto>> CreateRoom(CreateRoomRequest request)
    {
        var result = await roomManager.CreateRoomAsync(request.userId, request.privacy, request.password, request.maxPlayers);
        
        if (result.Success)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}");
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }
        
        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<DataResponse<RoomDto>> JoinRoom(JoinRoomRequest request)
    {
        var result = await roomManager.JoinRoomAsync(request.userId, request.roomId, request.password);

        if (result.Success)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{request.roomId}");
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }

        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> LeaveRoom(LeaveRoomRequest request)
    {
        var result = await roomManager.LeaveRoomAsync(request.userId, request.roomId);
        
        if (result.Success)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{request.roomId}");
            return EmptyResponse.CreateSuccess();
        }
        
        return EmptyResponse.CreateFailure(result.ErrorMsg);
    }

    public async Task<DataResponse<RoomDto>> FindQuickRoom(FindQuickRoomRequest request)
    {
        var result = await roomManager.FindOrCreateQuickRoomAsync(request.userId);

        if (result.Success)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}");
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }

        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }
}