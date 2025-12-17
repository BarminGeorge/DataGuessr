using Application.Extensions;
using Application.Interfaces;
using Application.Requests;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Application.Endpoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/rooms");
            
        group.AddFluentValidationAutoValidation();
        
        group.MapGet("{inviteCode}/privacy", GetRoomPrivacy);
        
        return app;
    }

    private static async Task<IResult> GetRoomPrivacy([AsParameters] GetRoomPrivacyRequest request,
        [FromServices] IRoomManager roomManager, CancellationToken ct)
    {
        return (await roomManager.GetRoomPrivacyAsync(request.InviteCode, ct))
            .ToResult();
    }
}