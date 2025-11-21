using Application.DtoUI;
using Application.Mappers;
using Application.Requests_Responses;

namespace Application.Endpoints.Hubs;

public partial class AppHub
{
    public async Task<DataResponse<GameDto>> CreateGame(CreateGameRequest request)
    {
        var result = await gameManager.CreateNewGameAsync(
            request.RoomId, 
            request.UserId, 
            request.Mode, 
            request.CountQuestions, 
            request.QuestionDuration, 
            request.Questions);

        return result.Success
            ? DataResponse<GameDto>.CreateSuccess(result.ResultObj.ToDto())
            : DataResponse<GameDto>.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> StartGame(StartGameRequest request)
    {
        var result = await gameManager.StartNewGameAsync(request.RoomId, request.UserId);
        return result.Success
            ? EmptyResponse.CreateSuccess()
            : EmptyResponse.CreateFailure(result.ErrorMsg);
    }

    public async Task<EmptyResponse> SubmitAnswer(SubmitAnswerRequest request)
    {
        var result = await gameManager.SubmitAnswerAsync(request.RoomId, request.Answer);
        return result.Success
            ? EmptyResponse.CreateSuccess()
            : EmptyResponse.CreateFailure(result.ErrorMsg);
    }
}