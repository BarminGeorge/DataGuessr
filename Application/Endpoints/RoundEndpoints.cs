using Application.Extensions;
using Application.Interfaces;
using Domain.ValueTypes;

namespace Application.EndPoints;

public static class RoundEndpoints
{
    public static IEndpointRouteBuilder MapRoundEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rooms/{roomId:guid}/games/{gameId:guid}/rounds");

        group.MapGet("current", GetCurrentRound);           // Текущий активный раунд
        group.MapPost("{roundId:guid}/answer", SubmitAnswer); // Ответ на раунд
        group.MapGet("{roundId:guid}/results", GetRoundResults); // Результаты ОДНОГО раунда
        group.MapGet("leaderboard", GetGameLeaderboard);    // Итоговый лидерборд всей игры
        group.MapGet("status", GetGameStatus);              // Статус игры (активна/завершена)

        return app;
    }

    private static async Task<IResult> GetCurrentRound(
        Guid roomId,
        Guid gameId,
        IRoundService roundService)
    {
        var currentRound = await roundService.GetCurrentRoundAsync(roomId, gameId);
        return currentRound != null ? Results.Ok(currentRound) : Results.NotFound("Активных раундов нет");
    }

    private static async Task<IResult> SubmitAnswer(
        SubmitAnswerRequest request,
        Guid roomId,
        Guid gameId,
        Guid roundId,
        IRoundService roundService,
        HttpContext context)
    {
        var userId = context.GetUserId();
        var result = await roundService.SubmitAnswerAsync(roomId, gameId, roundId, userId, request.Answer);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetRoundResults(
        Guid roomId,
        Guid gameId,
        Guid roundId,
        IRoundService roundService)
    {
        var results = await roundService.GetRoundResultsAsync(roomId, gameId, roundId);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetGameLeaderboard(
        Guid roomId,
        Guid gameId,
        IRoundService roundService)
    {
        var leaderboard = await roundService.GetGameLeaderboardAsync(roomId, gameId);
        return Results.Ok(leaderboard);
    }

    private static async Task<IResult> GetGameStatus(
        Guid roomId,
        Guid gameId,
        IRoundService roundService)
    {
        var status = await roundService.GetGameStatusAsync(roomId, gameId);
        return Results.Ok(status);
    }
}

public record SubmitAnswerRequest(Answer Answer);