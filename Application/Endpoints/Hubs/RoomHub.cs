using Application.DtoUI;
using Application.Extensions;
using Application.Mappers;
using Application.Requests;
using Domain.Common;

namespace Application.Endpoints.Hubs;

public partial class AppHub
{
    public async Task<OperationResult<RoomDto>> CreateRoom(CreateRoomRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult<RoomDto>.Error.Validation(error);
        
        var result = await roomManager.CreateRoomAsync(request.UserId, request.Privacy, ct, request.Password, request.MaxPlayers);
        
        if (result is { Success: true, ResultObj: not null })
        {
            var connectionServiceResult = await connectionService.AddConnection(Context.ConnectionId, request.UserId, result.ResultObj.Id, ct);
            if (!connectionServiceResult.Success)
                return connectionServiceResult.ConvertToOperationResult<RoomDto>();

            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}", ct);
            
            var joinOperation = () => roomManager.JoinRoomAsync(result.ResultObj.Id, request.UserId, ct, request.Password);
            var joinResult = await joinOperation.WithRetry(delay: TimeSpan.FromSeconds(0.15));
            if (!joinResult.Success || joinResult.ResultObj is null)
                return joinResult.ConvertToOperationResult<RoomDto>();
            
            return OperationResult<RoomDto>.Ok(joinResult.ResultObj.ToDto());
        }
        
        return result.ConvertToOperationResult<RoomDto>();
    }

    public async Task<OperationResult<RoomDto>> JoinRoom(JoinRoomRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult<RoomDto>.Error.Validation(error);
        
        var result = await roomManager.JoinRoomAsync(request.UserId, request.RoomId, ct, request.Password);

        if (result is { Success: true, ResultObj: not null})
        {
            await connectionService.AddConnection(Context.ConnectionId, request.UserId, request.RoomId, ct);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{request.RoomId}", ct);
            return OperationResult<RoomDto>.Ok(result.ResultObj.ToDto());
        }

        return result.ConvertToOperationResult<RoomDto>();
    }

    public async Task<OperationResult> LeaveRoom(LeaveRoomRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult.Error.Validation(error);
        
        var result = await roomManager.LeaveRoomAsync(request.RoomId, request.UserId, ct);
        
        if (result.Success)
        {
            await connectionService.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{request.RoomId}", ct);
            return OperationResult.Ok();
        }
        
        return result;
    }

    public async Task<OperationResult<RoomDto>> FindQuickRoom(FindQuickRoomRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult<RoomDto>.Error.Validation(error);
        
        var result = await roomManager.FindOrCreateQuickRoomAsync(request.UserId, ct);

        if (result is { Success: true, ResultObj: not null })
        {
            await connectionService.AddConnection(Context.ConnectionId, request.UserId, result.ResultObj.Id, ct);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{result.ResultObj.Id}", ct);
            return OperationResult<RoomDto>.Ok(result.ResultObj.ToDto());
        }

        return result.ConvertToOperationResult<RoomDto>();
    }

    public async Task<OperationResult> KickPlayerFromRoom(KickPlayerRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult.Error.Validation(error);
        
        var result = await roomManager.KickPlayerFromRoom(request.UserId,  request.RoomId, request.RemovedPlayerId, ct);
        
        if (result.Success)
        {
            var getConnectionResult = await connectionService.GetConnectionIdByPlayer(request.RemovedPlayerId);
            if (getConnectionResult is { Success: true, ResultObj: not null })
            {
                var operation = () => connectionService.RemoveConnection(getConnectionResult.ResultObj);
                var removeConnectionResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.2));
                if (!removeConnectionResult.Success)
                    return removeConnectionResult;
                
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{request.RoomId}", ct);
                
                return OperationResult.Ok();
            }
        }

        return result;
    }
}