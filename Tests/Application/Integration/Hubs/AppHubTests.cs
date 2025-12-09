using Domain.Common; 

using FakeItEasy;


namespace Tests.Application.Integration.Hubs;

public class AppHubTests : HubTests
{
    [Test]
    public async Task OnDisconnected_WhenPlayerExists_ShouldRemoveConnectionAndLeaveRoom()
    {
        var playerId = Guid.NewGuid();
        var roomId = Guid.NewGuid();


        A.CallTo(() => ConnectionServiceFake.GetPlayerByConnection(A<string>._))
            .Returns(OperationResult<(Guid, Guid)>.Ok((playerId, roomId)));

        A.CallTo(() => ConnectionServiceFake.RemoveConnection(A<string>._))
            .Returns(OperationResult.Ok());

        A.CallTo(() => RoomManagerFake.LeaveRoomAsync(roomId, playerId, A<CancellationToken>._))
            .Returns(OperationResult.Ok());


        await HubConnection.StartAsync();
        var connectionId = HubConnection.ConnectionId;
        
 
        await HubConnection.StopAsync(); 
        
        await Task.Delay(100); 
        Assert.That(connectionId, Is.Not.Null);
        Assert.Multiple(() =>
        {
            A.CallTo(() => ConnectionServiceFake.GetPlayerByConnection(connectionId))
                .MustHaveHappenedOnceExactly();
            
            A.CallTo(() => ConnectionServiceFake.RemoveConnection(connectionId))
                .MustHaveHappenedOnceExactly();
            
            A.CallTo(() => RoomManagerFake.LeaveRoomAsync(roomId, playerId, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        });
    }

    [Test]
    public async Task OnDisconnected_WhenPlayerNotFound_ShouldNotCallLeaveRoom()
    {
        A.CallTo(() => ConnectionServiceFake.GetPlayerByConnection(A<string>._))
            .Returns(OperationResult<(Guid, Guid)>.Error);


        await HubConnection.StartAsync();
        var connectionId = HubConnection.ConnectionId;
        
        await HubConnection.StopAsync();
        await Task.Delay(100); 

        Assert.Multiple(() =>
        {
            A.CallTo(() => ConnectionServiceFake.GetPlayerByConnection(connectionId!))
                .MustHaveHappenedOnceExactly();
            
            A.CallTo(() => ConnectionServiceFake.RemoveConnection(A<string>._))
                .MustNotHaveHappened();

            A.CallTo(() => RoomManagerFake.LeaveRoomAsync(A<Guid>._, A<Guid>._, A<CancellationToken>._))
                .MustNotHaveHappened();
        });
    }
}