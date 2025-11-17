using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Application.Services;

namespace Application.EndPoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("rooms");

        group.MapPost("", CreateRoom);
        group.MapGet("", GetAvailableRooms);
        group.MapPost("{roomId:guid}/join", JoinRoom);
        group.MapPost("{roomId:guid}/leave}", LeaveRoom);
        group.MapGet("quick-room", FindQuickRoom);

        return app;
    }

    private static async Task<IResult> CreateRoom(CreateRoomRequest request, IRoomManager roomManager,
        HttpContext context)
    {
        var userId = GetUserIdFromContext(context);
        var room = await roomManager.CreateRoomAsync(userId, request.privacy, request.password, request.maxPlayers);
        return Results.Ok(room);
    }

    private static async Task<IResult> GetAvailableRooms(CreateRoomRequest request, IRoomManager roomManager,
        HttpContext context)
    {
        var rooms = await roomManager.GetAvailablePublicRoomsAsync();
        return Results.Ok(rooms);
    }

    private static async Task<IResult> JoinRoom(Guid roomId, JoinRoomRequest request, IRoomManager roomManager,
        HttpContext context)
    {
        var userId = GetUserIdFromContext(context);
        var result = await roomManager.JoinRoomAsync(roomId, userId, request.password);
        return Results.Ok(result);
    }

    private static async Task<IResult> LeaveRoom(Guid roomId, IRoomManager roomManager, HttpContext context)
    {
        var userId = GetUserIdFromContext(context);
        var result = await roomManager.LeaveRoomAsync(roomId, userId);
        return Results.Ok(result);
    }

    private static async Task<IResult> FindQuickRoom(QuickRoomService roomService, HttpContext context)
    {
        var userId = GetUserIdFromContext(context);
        var room = await roomService.FindOrCreateQuickRoomAsync(userId);
        return Results.Ok(room);
    }

    private static Guid GetUserIdFromContext(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst("userId");
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;
        throw new UnauthorizedAccessException("User not authenticated");
    }
}

public record CreateRoomRequest([Required] RoomPrivacy privacy, string? password, int maxPlayers);

public record JoinRoomRequest(string? password);