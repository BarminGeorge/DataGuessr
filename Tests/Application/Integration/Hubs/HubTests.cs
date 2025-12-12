using Application.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tests.Application.Integration.Hubs;

public abstract class HubTests
{
    private WebApplicationFactory<TestEntryPoint> factory;
    protected HubConnection HubConnection;
    protected IRoomManager RoomManagerFake;
    protected IGameManager GameManagerFake;
    protected IConnectionService ConnectionServiceFake;
    protected IQuestionService QuestionServiceFake;

    [SetUp]
    public void Setup()
    {
        RoomManagerFake = A.Fake<IRoomManager>();
        GameManagerFake = A.Fake<IGameManager>();
        ConnectionServiceFake = A.Fake<IConnectionService>();
        QuestionServiceFake = A.Fake<IQuestionService>();
        factory = new WebApplicationFactory<TestEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(RoomManagerFake);
                    services.AddSingleton(GameManagerFake);
                    services.AddSingleton(ConnectionServiceFake);
                    services.AddSingleton(QuestionServiceFake);

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
        HubConnection = new HubConnectionBuilder()
                           .WithUrl("http://localhost/appHub", options => 
                               options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler())
                           .WithAutomaticReconnect()
                           .Build();
    }
    
    [TearDown]
    public void TearDownSingle()
    {
        factory.Dispose();
        HubConnection.DisposeAsync();
    }
}