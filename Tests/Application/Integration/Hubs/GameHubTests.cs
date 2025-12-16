using Application.DtoUI;
using Application.Requests;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;
using FakeItEasy;
using Microsoft.AspNetCore.SignalR.Client;

namespace Tests.Application.Integration.Hubs;

[TestFixture]
public class GameHubTests: HubTests
{
    private Guid userId = Guid.NewGuid();
    private Guid roomId = Guid.NewGuid();
    private GameMode gameMode = GameMode.BoolMode;
    private Guid gameId = Guid.NewGuid();
    private Guid playerId = Guid.NewGuid();
    private Guid questionId = Guid.NewGuid();
    
    [Test]
    public async Task CreateGame_RequestWithoutQuestion_ReturnGame()
    {
        var questionsCount = 10;
        var questionDuraction = TimeSpan.FromMicroseconds(1);
        
        var game = new Game(roomId, gameMode, questionDuraction, questionsCount);
        A.CallTo(() =>
             GameManagerFake.CreateNewGameAsync(
                roomId,
                userId, 
                gameMode, 
                questionsCount,
                questionDuraction,  
                A<CancellationToken>._)).Returns(OperationResult<Game>.Ok(game));
        
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<GameDto>>("CreateGame", 
                new CreateGameRequest(userId, roomId, gameMode, questionsCount, questionDuraction.Seconds),
            CancellationToken.None);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj, Is.Not.Null);
            Assert.That(result.Success, Is.EqualTo(true));
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj.QuestionsCount, Is.EqualTo(questionsCount));
            Assert.That(result.ResultObj.QuestionDuration, Is.EqualTo(questionDuraction.Seconds));
            Assert.That(result.ResultObj.Mode, Is.EqualTo(gameMode));
            Assert.That(result.ResultObj.Status, Is.EqualTo(GameStatus.NotStarted));
        });
    }

    [Test]
    public async Task CreateGame_RequestWithIncorrectQuestionCount_ValidationError()
    {
        var invalidQuestionsCount = -1;
        var questionDuraction = TimeSpan.FromMicroseconds(1);
        
        var game = new Game(roomId, gameMode, questionDuraction, invalidQuestionsCount);
        A.CallTo(() =>
            GameManagerFake.CreateNewGameAsync(
                roomId,
                userId, 
                gameMode, 
                invalidQuestionsCount,
                questionDuraction,  
                A<CancellationToken>._)).Returns(OperationResult<Game>.Ok(game));
        
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<GameDto>>("CreateGame", 
            new CreateGameRequest(userId, roomId, gameMode, invalidQuestionsCount, questionDuraction.Seconds),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj, Is.Null);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Validation));
            Assert.That(result.Success, Is.EqualTo(false));
        });
    }

    [Test]
    public async Task StartGame_CorrectRequest_ReturnSuccess()
    {
        A.CallTo(() => GameManagerFake.StartNewGameAsync(roomId, userId, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult>("StartGame", new StartGameRequest(userId, roomId));
        
        Assert.That(result.Success, Is.EqualTo(true));
    }
    
    [Test]
    public async Task StartGame_IncorrectRequest_ReturnError()
    {
        A.CallTo(() => GameManagerFake.StartNewGameAsync(roomId, userId, A<CancellationToken>._))
            .Returns(OperationResult.Error);
        
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult>("StartGame", new StartGameRequest(userId, roomId));
        
        Assert.That(result.Success, Is.False);
    }
    
    [Test]
    public async Task SubmitAnswer_CorrectRequest_ReturnSuccess()
    {
        var answer = new BoolAnswer(true);
        
        A.CallTo(() => GameManagerFake.SubmitAnswerAsync(gameId, questionId, playerId, answer, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        A.CallTo(() => QuestionServiceFake.SubmitAnswerAsync(gameId, questionId, playerId, answer, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult>("SubmitAnswer", 
            new SubmitAnswerRequest(gameId, questionId, playerId, answer));
        
        Assert.That(result.Success, Is.True);
    }
    
    [Test]
    public async Task SubmitAnswer_WhenReturnError()
    {
        var answer = new BoolAnswer(true);
        
        A.CallTo(() => GameManagerFake.SubmitAnswerAsync(gameId, questionId, playerId, answer, A<CancellationToken>._))
            .Returns(OperationResult.Error);
        A.CallTo(() => QuestionServiceFake.SubmitAnswerAsync(gameId, questionId, playerId, answer, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult>("SubmitAnswer", 
            new SubmitAnswerRequest(gameId, questionId, playerId, answer));
        
        Assert.That(result.Success, Is.False);
    }
    
    [Test]
    public async Task FinishGame_CorrectRequest_ReturnRoom()
    {
        var room = new Room(userId, RoomPrivacy.Private, 10);
        A.CallTo(() => GameManagerFake.FinishGameAsync(userId, room.Id, A<CancellationToken>._))
            .Returns(new OperationResult<Room>(true, room));
                                                
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<RoomDto>>("FinishGame", new FinishGameRequest(userId, room.Id));
        
        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj, Is.Not.Null);
            Assert.That(result.Success, Is.EqualTo(true));
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj.Id, Is.EqualTo(room.Id));
            Assert.That(result.ResultObj.OwnerId, Is.EqualTo(userId));
        });
    }
    
    [Test]
    public async Task FinishGame_WhenReturnError()
    {
        var room = new Room(userId, RoomPrivacy.Private, 10);
        A.CallTo(() => GameManagerFake.FinishGameAsync(userId, room.Id, A<CancellationToken>._))
            .Returns(OperationResult<Room>.Error.NotFound("Room not found"));
                                                
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<RoomDto>>("FinishGame", new FinishGameRequest(userId, room.Id));
        
        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj, Is.Null);
            Assert.That(result.Success, Is.False);
        });
    }
}