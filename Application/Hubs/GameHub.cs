using Application.Dto;
using Application.Mappers;
using Application.Requests_Responses;

namespace Application.Hubs;

public partial class AppHub
{
    public async Task<DataResponse<GameDto>> CreateGame(CreateGameRequest request)
    {
        var result = await gameManager.CreateNewGameAsync(
            request.roomId, 
            request.userId, 
            request.mode, 
            request.countQuestions, 
            request.QuestionDuration, 
            request.questions);

        return result.Success
            ? DataResponse<GameDto>.CreateSuccess(result.ResultObj.ToDto())
            : DataResponse<GameDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> StartGame(StartGameRequest request)
    {
        var result = await gameManager.StartNewGameAsync(request.roomId, request.userId);
        return result.Success
            ? EmptyResponse.CreateSuccess()
            : EmptyResponse.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> SubmitAnswer(SubmitAnswerRequest request)
    {
        var result = await gameManager.SubmitAnswerAsync(request.roomId, request.Answer);
        return result.Success
            ? EmptyResponse.CreateSuccess()
            : EmptyResponse.CreateFailure(result.ErrorMsg);
    }
}