using Domain.ValueTypes;

namespace Application.EndPoints;

// TODO: доделать
public static class RoundEndpoints
{
    public static IEndpointRouteBuilder MapRoundEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rooms/{roomId:guid}/games/{matchId:guid}");

        group.MapGet("liderboard", GetStatistic);
        group.MapPost("answer", SaveAnswer);
        
        return app;
    }

    private static async Task<IResult> GetStatistic()
    {
        return Results.Ok();
    }
    
    private static async Task<IResult> SaveAnswer(SaveAnswerRequest request)
    {
        return Results.Ok();
    }
}

public record SaveAnswerRequest(Answer answer);