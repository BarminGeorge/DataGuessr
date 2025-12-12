using Application.DtoUI;
using Application.Extensions;
using Application.Interfaces;
using Application.Mappers;
using Domain.Common;
using Infrastructure.Interfaces;
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

    private static async Task<IResult> GetAvailableRooms([FromServices] IRoomManager roomManager, 
        [FromServices] IUserRepository userRepository, HttpContext context, CancellationToken ct)
    {
        var operationResult = await roomManager.GetAvailablePublicRoomsAsync(ct);
    
        if (operationResult is not { Success: true, ResultObj: not null })
            return operationResult.ToResult();
    
        var roomDtoTasks = operationResult.ResultObj
            .Select(room => room.ToDto(userRepository));
    
        var roomDtos = await Task.WhenAll(roomDtoTasks);
    
        return OperationResult<IEnumerable<RoomDto>>.Ok(roomDtos).ToResult();
    }

    private static async Task<IResult> GetRoomPrivacy([FromRoute] Guid roomId, 
        [FromServices] IRoomManager roomManager, HttpContext context, CancellationToken ct)
    {
        return (await roomManager.GetRoomPrivacyAsync(roomId, ct))
            .ToResult();
    }
}