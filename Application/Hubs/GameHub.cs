using Application.Requests_Responses;

namespace Application.Hubs;

public partial class AppHub
{
    public async Task<CreateGameResponse> CreateGame(CreateGameRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<StartGameResponse> StartGame(StartGameRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<SubmitAnswerResponse> SubmitAnswer(SubmitAnswerRequest request)
    {
        throw new NotImplementedException();
    }
}