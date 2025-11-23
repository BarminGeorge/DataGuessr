using Application.Interfaces;
using Application.Requests_Responses;

namespace Application.EndPoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("rooms");
        
        group.MapGet("", GetAvailableRooms);
        
        return app;
    }

    private static async Task<IResult> GetAvailableRooms(CreateRoomRequest request, IRoomManager roomManager,
        HttpContext context)
    {
        var operationResult = await roomManager.GetAvailablePublicRoomsAsync();
        return operationResult.Success ? Results.Ok(operationResult.ResultObj) : Results.BadRequest(operationResult);
    }
}