using Application.DtoUI;
using Application.Interfaces;
using Application.Requests;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tests.Application.Integration.Hubs;

public class GameHubTests
{
    private WebApplicationFactory<TestEntryPoint> factory;
    private IRoomManager roomManagerFake;
    private IGameManager gameManagerFake;
    private IConnectionService connectionServiceFake;
    
    private HubConnection hubConnection;

    [SetUp]
    public void Setup()
    {
        roomManagerFake = A.Fake<IRoomManager>();
        gameManagerFake = A.Fake<IGameManager>();
        connectionServiceFake = A.Fake<IConnectionService>();
        
        factory = new WebApplicationFactory<TestEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(roomManagerFake);
                    services.AddSingleton(gameManagerFake);
                    services.AddSingleton(connectionServiceFake);

                    services.AddSignalR(hubOptions =>
                    {
                        hubOptions.EnableDetailedErrors = true;
                    });
                });
                builder.ConfigureLogging(logging => 
                {
                    logging.ClearProviders(); 
                });

            });
        hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost/appHub", options => 
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler())
            .WithAutomaticReconnect()
            .Build();
    }

    [TearDown]
    public void TearDown()
    {
        factory.Dispose();
        hubConnection.DisposeAsync();
    }

    [Test]
    public async Task CreateGame_RequestWithoutQuestion_ReturnGame()
    {
        var userId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var gameMode = GameMode.DefaultMode;
        var questionsCount = 10;
        var questionDuraction = TimeSpan.FromMicroseconds(1);
        
        var game = new Game(roomId, gameMode, questionDuraction, questionsCount);
        A.CallTo(() =>
             gameManagerFake.CreateNewGameAsync(
                roomId,
                userId, 
                gameMode, 
                questionsCount,
                questionDuraction,  
                A<CancellationToken>._, null)).Returns(OperationResult<Game>.Ok(game));
        
        await hubConnection.StartAsync();
        var result = await hubConnection.InvokeAsync<OperationResult<GameDto>>("CreateGame", 
                new CreateGameRequest(userId, roomId, gameMode, questionsCount, questionDuraction),
            CancellationToken.None);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj, Is.Not.Null);
            Assert.That(result.Success, Is.EqualTo(true));
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj.QuestionsCount, Is.EqualTo(questionsCount));
            Assert.That(result.ResultObj.QuestionDuration, Is.EqualTo(questionDuraction));
            Assert.That(result.ResultObj.Mode, Is.EqualTo(gameMode));
            Assert.That(result.ResultObj.Status, Is.EqualTo(GameStatus.NotStarted));
        });
    }

    [Test]
    public async Task CreateGame_RequestWithIncorrectQuestionCount_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var gameMode = GameMode.DefaultMode;
        var invalidQuestionsCount = -1;
        var questionDuraction = TimeSpan.FromMicroseconds(1);
        
        var game = new Game(roomId, gameMode, questionDuraction, invalidQuestionsCount);
        A.CallTo(() =>
            gameManagerFake.CreateNewGameAsync(
                roomId,
                userId, 
                gameMode, 
                invalidQuestionsCount,
                questionDuraction,  
                A<CancellationToken>._, null)).Returns(OperationResult<Game>.Ok(game));
        
        await hubConnection.StartAsync();
        var result = await hubConnection.InvokeAsync<OperationResult<GameDto>>("CreateGame", 
            new CreateGameRequest(userId, roomId, gameMode, invalidQuestionsCount, questionDuraction),
            CancellationToken.None);
        Console.WriteLine(result);
        Assert.Multiple(() =>
        {
            Assert.That(result.ResultObj, Is.Null);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Validation));
            Assert.That(result.Success, Is.EqualTo(false));
        });
    }

    [Test]
    public async Task StartGame_CorrectRequest_StartsGame()
    {
        var userId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        A.CallTo(() => gameManagerFake.StartNewGameAsync(roomId, userId, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        
        await hubConnection.StartAsync();
        var result = await hubConnection.InvokeAsync<OperationResult>("StartGame", new StartGameRequest(userId, roomId));
        Console.WriteLine(result);
    }
}