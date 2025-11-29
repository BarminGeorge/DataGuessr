using Application.DtoUI;
using Application.Mappers;
using Application.Requests_Responses;

namespace Application.Endpoints.Hubs;

public partial class AppHub
{
    public async Task<DataResponse<GameDto>> CreateGame(CreateGameRequest request, CancellationToken ct = default)
    {
        var result = await gameManager.CreateNewGameAsync(
            request.RoomId, 
            request.UserId, 
            request.Mode, 
            request.CountQuestions, 
            request.QuestionDuration, 
            ct,
            request.Questions);

        return result.Success
            ? DataResponse<GameDto>.CreateSuccess(result.ResultObj.ToDto())
            : DataResponse<GameDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> StartGame(StartGameRequest request, CancellationToken ct = default)
    {
        var result = await gameManager.StartNewGameAsync(request.RoomId, request.UserId, ct);
        return result.Success
            ? EmptyResponse.CreateSuccess()
            : EmptyResponse.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> SubmitAnswer(SubmitAnswerRequest request, CancellationToken ct = default)
    {
        var result = await gameManager.SubmitAnswerAsync(request.RoomId, request.GameId, request.QuestionId, request.Answer, ct);
        return result.Success
            ? EmptyResponse.CreateSuccess()
            : EmptyResponse.CreateFailure(result.ErrorMsg);
    }

    public async Task<DataResponse<RoomDto>> FinishGame(FinishGameRequest request, CancellationToken ct = default)
    {
        var result = await gameManager.FinishGameAsync(request.userId, request.roomId, ct);
        return result.Success
            ? DataResponse<RoomDto>.CreateSuccess(result.ResultObj.ToDto())
            : DataResponse<RoomDto>.CreateFailure(result.ErrorMsg);
    }
}

public record FinishGameRequest(Guid userId, Guid roomId);