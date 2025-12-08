using Application.DtoUI;
using Application.Extensions;
using Application.Interfaces;
using Application.Mappers;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Application.EndPoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/rooms");
            
        group.AddFluentValidationAutoValidation();
        
        group.MapGet("", GetAvailableRooms);
        group.MapGet("{roomId:guid}", GetRoomPrivacy);
        
        return app;
    }

    private static async Task<IResult> GetAvailableRooms(IRoomManager roomManager, 
        HttpContext context, CancellationToken ct)
    {
        var operationResult = await roomManager.GetAvailablePublicRoomsAsync(ct);
        return operationResult is { Success: true, ResultObj: not null }
            ? OperationResult<IEnumerable<RoomDto>>.Ok(operationResult.ResultObj
                .Select(x => x.ToDto()))
                .ToResult() 
            : operationResult.ToResult();
    }

    private static async Task<IResult> GetRoomPrivacy([FromRoute] Guid roomId, 
        IRoomManager roomManager, HttpContext context, CancellationToken ct)
    {
        return (await roomManager.GetRoomPrivacyAsync(roomId, ct))
            .ToResult();
    }
}