using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Application.EndPoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("rooms");
        
        group.MapGet("", GetAvailableRooms);
        group.MapGet("{roomId:guid}", GetRoomPrivacy);
        
        return app;
    }

    private static async Task<IResult> GetAvailableRooms(IRoomManager roomManager, HttpContext context, CancellationToken ct)
    {
        var operationResult = await roomManager.GetAvailablePublicRoomsAsync(ct);
        return operationResult.Success ? Results.Ok(operationResult.ResultObj) : Results.BadRequest(operationResult);
    }

    private static async Task<IResult> GetRoomPrivacy([FromRoute] Guid roomId, IRoomManager roomManager, HttpContext context, CancellationToken ct)
    {
        var result = await roomManager.GetRoomPrivacyAsync(roomId, ct);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}