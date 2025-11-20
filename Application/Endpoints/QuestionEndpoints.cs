using Application.Extensions;
using Application.Interfaces;
using Application.Requests;

namespace Application.EndPoints;

public static class QuestionEndpoints
{
    public static IEndpointRouteBuilder MapRoundEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rooms/{roomId:guid}/games/{gameId:guid}/rounds");

        group.MapGet("current", GetCurrentQuestion);
        group.MapPost("{questionId:guid}/answer", SubmitAnswer);
        group.MapGet("{questionId:guid}/results", GetQuestionResults);
        group.MapGet("leaderboard", GetGameLeaderboard);
        group.MapGet("status", GetGameStatus);              // Статус игры (активна/завершена)

        return app;
    }

    private static async Task<IResult> GetCurrentQuestion(
        Guid roomId,
        Guid gameId,
        IQuestionService questionService)
    {
        var currentRound = await questionService.GetCurrentQuestionAsync(roomId, gameId);
        return currentRound != null ? Results.Ok(currentRound) : Results.NotFound("Активных раундов нет");
    }

    private static async Task<IResult> SubmitAnswer(
        SubmitAnswerRequest request,
        Guid roomId,
        Guid gameId,
        Guid roundId,
        IQuestionService questionService,
        HttpContext context)
    {
        var userId = context.GetUserId();
        var result = await questionService.SubmitAnswerAsync(roomId, gameId, roundId, userId, request.Answer);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetQuestionResults(
        Guid roomId,
        Guid gameId,
        Guid roundId,
        IQuestionService questionService)
    {
        var results = await questionService.GetQuestionResultsAsync(roomId, gameId, roundId);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetGameLeaderboard(
        Guid roomId,
        Guid gameId,
        IQuestionService questionService)
    {
        var leaderboard = await questionService.GetGameLeaderboardAsync(roomId, gameId);
        return Results.Ok(leaderboard);
    }

    private static async Task<IResult> GetGameStatus(
        Guid roomId,
        Guid gameId,
        IQuestionService questionService)
    {
        var status = await questionService.GetGameStatusAsync(roomId, gameId);
        return Results.Ok(status);
    }
}