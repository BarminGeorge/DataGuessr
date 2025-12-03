using Application.DtoUI;
using Application.Mappers;
using Application.Requests_Responses;
using Domain.Common;

namespace Application.Endpoints.Hubs;

public partial class AppHub
{
    public async Task<DataResponse<RoomDto>> CreateRoom(CreateRoomRequest request, CancellationToken ct = default)
    {
        var result = await roomManager.CreateRoomAsync(request.UserId, request.Privacy, ct, request.Password, request.MaxPlayers);
        
        if (result is { Success: true, ResultObj: not null })
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

        if (result is { Success: true, ResultObj: not null})
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

        if (result is { Success: true, ResultObj: not null })
        {
            await connectionService.AddConnection(Context.ConnectionId, request.UserId, result.ResultObj.Id, ct);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}", ct);
            return DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto());
        }

        return DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> KickPlayerFromRoom(KickPlayerRequest request, CancellationToken ct = default)
    {
        var result = await roomManager.KickPlayerFromRoom(request.UserId,  request.RoomId, request.RemovedPlayerId, ct);
        
        if (result.Success)
        {
            var getConnectionResult = await connectionService.GetConnectionIdByPlayer(request.RemovedPlayerId);
            if (getConnectionResult is { Success: true, ResultObj: not null })
            {
                var operation = () => connectionService.RemoveConnection(getConnectionResult.ResultObj);
                var removeConnectionResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.2));
                if (!removeConnectionResult.Success)
                    return EmptyResponse.CreateFailure(removeConnectionResult.ErrorMsg);
                
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{request.RoomId}", ct);
                
                return EmptyResponse.CreateSuccess();
            }
        }
        
        return EmptyResponse.CreateFailure(result.ErrorMsg);
    }
}