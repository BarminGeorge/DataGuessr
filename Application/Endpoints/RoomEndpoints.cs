using Application.Extensions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Application.EndPoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/rooms");
            
        group.AddFluentValidationAutoValidation();
        
        group.MapGet("{roomId:guid}", GetRoomPrivacy);
        
        return app;
    }

    private static async Task<IResult> GetRoomPrivacy([FromRoute] Guid roomId, 
        [FromServices] IRoomManager roomManager, HttpContext context, CancellationToken ct)
    {
        return (await roomManager.GetRoomPrivacyAsync(roomId, ct))
            .ToResult();
    }
}