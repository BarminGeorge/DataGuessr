using Application.DtoUI;
using Application.Extensions;
using Application.Mappers;
using Application.Requests;
using Domain.Common;

namespace Application.Endpoints.Hubs;

public partial class AppHub
{
    public async Task<OperationResult<GameDto>> CreateGame(CreateGameRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult<GameDto>.Error.Validation(error);
        
        var result = await gameManager.CreateNewGameAsync(
            request.RoomId,
            request.UserId,
            request.Mode,
            request.CountQuestions,
            TimeSpan.FromSeconds(request.QuestionDuration),
            ct);

        return result is { Success: true, ResultObj: not null }
            ? OperationResult<GameDto>.Ok(result.ResultObj.ToDto())
            : result.ConvertToOperationResult<GameDto>();
    }

    public async Task<OperationResult> StartGame(StartGameRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult.Error.Validation(error);

        return await gameManager.StartNewGameAsync(request.RoomId, request.UserId, ct);
    }

    public async Task<OperationResult> SubmitAnswer(SubmitAnswerRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult.Error.Validation(error);

        return await gameManager.SubmitAnswerAsync(request.GameId, request.QuestionId, request.PlayerId, request.Answer, ct);
    }

    public async Task<OperationResult<RoomDto>> FinishGame(FinishGameRequest request)
    {
        var ct = Context.ConnectionAborted;
        if (await this.ValidateRequestAsync(request, ct) is { } error)
            return OperationResult<RoomDto>.Error.Validation(error);

        var result = await gameManager.FinishGameAsync(request.UserId, request.RoomId, ct);
        return result is { Success: true, ResultObj: not null }
            ? OperationResult<RoomDto>.Ok(result.ResultObj.ToDto())
            : result.ConvertToOperationResult<RoomDto>();
    }
}