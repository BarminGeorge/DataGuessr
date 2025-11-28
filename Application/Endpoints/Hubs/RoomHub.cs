using Application.DtoUI;
using Application.Mappers;
using Application.Requests_Responses;

namespace Application.Endpoints.Hubs;

public partial class AppHub
{
    public async Task<DataResponse<RoomDto>> CreateRoom(CreateRoomRequest request, CancellationToken ct = default)
    {
        var result = await roomManager.CreateRoomAsync(request.UserId, request.Privacy, ct, request.Password, request.MaxPlayers);
        
        if (result.Success)
        {
            await connectionService.AddConnection(Context.ConnectionId, request.UserId, result.ResultObj.Id, ct);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}", ct);
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }
        
        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<DataResponse<RoomDto>> JoinRoom(JoinRoomRequest request, CancellationToken ct = default)
    {
        var result = await roomManager.JoinRoomAsync(request.UserId, request.RoomId, ct, request.Password);

        if (result.Success)
        {
            await connectionService.AddConnection(Context.ConnectionId, request.UserId, request.RoomId, ct);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{request.RoomId}", ct);
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }

        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> LeaveRoom(LeaveRoomRequest request, CancellationToken ct = default)
    {
        var result = await roomManager.LeaveRoomAsync(request.UserId, request.RoomId, ct);
        
        if (result.Success)
        {
            await connectionService.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{request.RoomId}", ct);
            return EmptyResponse.CreateSuccess();
        }
        
        return EmptyResponse.CreateFailure(result.ErrorMsg);
    }

    public async Task<DataResponse<RoomDto>> FindQuickRoom(FindQuickRoomRequest request, CancellationToken ct = default)
    {
        var result = await roomManager.FindOrCreateQuickRoomAsync(request.UserId, ct);

        if (result.Success)
        {
            await connectionService.AddConnection(Context.ConnectionId, request.UserId, result.ResultObj.Id, ct);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}", ct);
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }

        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }
}