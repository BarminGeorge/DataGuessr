using Application.DtoUI;
using Application.Interfaces;
using Application.Mappers;
using Domain.Common;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Application.EndPoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("rooms");
        
        group.AddFluentValidationAutoValidation();
        
        group.MapGet("", GetAvailableRooms);
        group.MapGet("{roomId:guid}", GetRoomPrivacy);
        
        return app;
    }

    private static async Task<OperationResult<IEnumerable<RoomDto>>> GetAvailableRooms(IRoomManager roomManager, 
        HttpContext context, CancellationToken ct)
    {
        var operationResult = await roomManager.GetAvailablePublicRoomsAsync(ct);
        return operationResult.Success 
            ? OperationResult<IEnumerable<RoomDto>>.Ok(operationResult.ResultObj
                .Select(x => x.ToDto())) 
            : OperationResult<IEnumerable<RoomDto>>.Error(operationResult.ErrorMsg);
    }

    private static async Task<OperationResult<RoomPrivacy>> GetRoomPrivacy([FromRoute] Guid roomId, 
        IRoomManager roomManager, HttpContext context, CancellationToken ct)
    {
        return await roomManager.GetRoomPrivacyAsync(roomId, ct);
    }
}