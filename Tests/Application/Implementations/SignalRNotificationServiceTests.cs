using Application.Endpoints.Hubs;
using Application.Notifications;
using Application.Services;
using FakeItEasy;
using Microsoft.AspNetCore.SignalR;

namespace Tests.Application.Implementations;

[TestFixture]
public class SignalRNotificationServiceTests
{
    private IHubContext<AppHub> fakeHubContext;
    private IHubClients fakeClients;
    private IClientProxy fakeClientProxy;
    private SignalRNotificationService service;

    [SetUp]
    public void Setup()
    {
        fakeHubContext = A.Fake<IHubContext<AppHub>>();
        fakeClients = A.Fake<IHubClients>();
        fakeClientProxy = A.Fake<IClientProxy>();

        A.CallTo(() => fakeHubContext.Clients).Returns(fakeClients);
        A.CallTo(() => fakeClients.Group(A<string>._)).Returns(fakeClientProxy);

        service = new SignalRNotificationService(fakeHubContext);
    }

    [Test]
    public async Task NotifyGameRoomAsync_ValidNotification_CallsSendAsync()
    {
        var roomId = Guid.NewGuid();
        var notification = new TestNotification("notification");

        A.CallTo(() => fakeClientProxy.SendCoreAsync(
                notification.MethodName,
                A<object[]>.That.Matches(args => 
                    args.Length == 1 && args[0] == notification),
                A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        var result = await service.NotifyGameRoomAsync(roomId, notification);

        Assert.That(result.Success, Is.True);
        
        var expectedGroupName = $"room-{roomId}";
        A.CallTo(() => fakeClients.Group(expectedGroupName))
            .MustHaveHappenedOnceExactly();
            
        A.CallTo(() => fakeClientProxy.SendCoreAsync(
                notification.MethodName,
                A<object[]>.That.Matches(args => 
                    args.Length == 1 && ReferenceEquals(args[0], notification)),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task NotifyGameRoomAsync_WhenSendAsyncThrows_ReturnsError()
    {
        var roomId = Guid.NewGuid();
        var notification = new TestNotification("notification");
            
        var exception = new InvalidOperationException("SignalR error");
            
        A.CallTo(() => fakeClientProxy.SendCoreAsync(
                A<string>._,
                A<object[]>._,
                A<CancellationToken>._))
            .ThrowsAsync(exception);

        var result = await service.NotifyGameRoomAsync(roomId, notification);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain("SignalR error"));
        });
    }
}

internal record TestNotification(string Data) : GameNotification
{
    public override string MethodName => "TestMethod";
}