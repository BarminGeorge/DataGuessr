using Application.Extensions;
using Application.Interfaces;
using Application.Requests_Responses;
using Application.Services;

namespace Application.EndPoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("rooms");
        
        group.MapGet("", GetAvailableRooms);
        group.MapGet("quick-room", FindQuickRoom);

        return app;
    }

    private static async Task<IResult> GetAvailableRooms(CreateRoomRequest request, IRoomManager roomManager,
        HttpContext context)
    {
        var rooms = await roomManager.GetAvailablePublicRoomsAsync();
        return Results.Ok(rooms);
    }

    private static async Task<IResult> FindQuickRoom(QuickRoomService roomService, HttpContext context)
    {
        var userId = context.GetUserId();
        var room = await roomService.FindOrCreateQuickRoomAsync(userId);
        return Results.Ok(room);
    }
}