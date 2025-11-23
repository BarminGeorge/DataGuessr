using Application.DtoUI;
using Application.Mappers;
using Application.Requests_Responses;

namespace Application.Endpoints.Hubs;

public partial class AppHub
{
    public async Task<DataResponse<RoomDto>> CreateRoom(CreateRoomRequest request)
    {
        var result = await roomManager.CreateRoomAsync(request.UserId, request.Privacy, request.Password, request.MaxPlayers);
        
        if (result.Success)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}");
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }
        
        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<DataResponse<RoomDto>> JoinRoom(JoinRoomRequest request)
    {
        var result = await roomManager.JoinRoomAsync(request.UserId, request.RoomId, request.Password);

        if (result.Success)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{request.RoomId}");
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }

        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> LeaveRoom(LeaveRoomRequest request)
    {
        var result = await roomManager.LeaveRoomAsync(request.UserId, request.RoomId);
        
        if (result.Success)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{request.RoomId}");
            return EmptyResponse.CreateSuccess();
        }
        
        return EmptyResponse.CreateFailure(result.ErrorMsg);
    }

    public async Task<DataResponse<RoomDto>> FindQuickRoom(FindQuickRoomRequest request)
    {
        var result = await roomManager.FindOrCreateQuickRoomAsync(request.UserId);

        if (result.Success)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}");
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }

        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }
}