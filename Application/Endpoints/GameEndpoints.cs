using Application.Extensions;
using Application.Interfaces;
using Application.Requests;

namespace Application.EndPoints;

public static class GameEndpoints
{
    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("rooms/{roomId:guid}/games");

        group.MapPost("", CreateGame);
        group.MapPost("start", StartGame);

        return app;
    }

    private static async Task<IResult> CreateGame(GameCreateRequest request, Guid roomId, IGameManager manager, HttpContext context)
    {
        var userId = context.GetUserId();
        var game = await manager.CreateNewGameAsync(roomId, userId, request.Mode);
        return Results.Ok(game);
    }

    private static async Task<IResult> StartGame(Guid roomId, IGameManager manager, HttpContext context)
    {
        var userId = context.GetUserId();
        var game = await manager.StartNewGameAsync(roomId, userId);
        return Results.Ok(game);
    }
}