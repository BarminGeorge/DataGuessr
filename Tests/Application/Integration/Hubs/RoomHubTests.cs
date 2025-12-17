using Application.DtoUI;
using Application.Requests;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Microsoft.AspNetCore.SignalR.Client;

namespace Tests.Application.Integration.Hubs;

public class RoomHubTests: HubTests
{
    private readonly Guid userId = Guid.NewGuid();
    private readonly int maxPlayers = 10;
    private const string inviteCode = "ABC245";

    [Test]
    public async Task CreateRoom_WithPassword_ShouldReturnRoom()
    {
        var room = new Room(userId, RoomPrivacy.Private, maxPlayers, "password12P");

        A.CallTo(() =>
                RoomManagerFake.CreateRoomAsync(userId, room.Privacy, A<CancellationToken>._, room.Password,
                    maxPlayers))
            .Returns(new OperationResult<Room>(true, room));
        A.CallTo(() => RoomManagerFake.JoinRoomAsync(room.Id, userId, A<CancellationToken>._, A<string>._))
            .Returns(new OperationResult<Room>(true, room));
        A.CallTo(() => ConnectionServiceFake.AddConnection(A<string>._, userId, room.Id, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        
        var requestWithPassword = new CreateRoomRequest(userId, RoomPrivacy.Private, room.Password, maxPlayers);
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<RoomDto>>("CreateRoom", requestWithPassword);
        Console.WriteLine(result);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj, Is.Not.Null);
        });
        
        Assert.That(result.ResultObj.OwnerId, Is.EqualTo(userId));
        
        A.CallTo(() =>
            RoomManagerFake.CreateRoomAsync(userId, room.Privacy, A<CancellationToken>._, room.Password,
                maxPlayers)).MustHaveHappenedOnceExactly();
        A.CallTo(() =>
            ConnectionServiceFake.AddConnection(A<string>._, userId, room.Id, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public async Task CreateRoom_WithValidationError_ShouldReturnValidationErrorMessage()
    {
        var requestWithoutPassword = new CreateRoomRequest(userId, RoomPrivacy.Private, null, maxPlayers);
        
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<RoomDto>>("CreateRoom", requestWithoutPassword);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ResultObj, Is.Null);
        });

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Validation));
        A.CallTo(() =>
        RoomManagerFake.CreateRoomAsync(A<Guid>._, A<RoomPrivacy>._, A<CancellationToken>._, A<string>._,
                A<int>._)).MustNotHaveHappened();
        
        A.CallTo(() =>
            ConnectionServiceFake.AddConnection(A<string>._, A<Guid>._, A<Guid>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Test]
    public async Task CreateRoom_WithGameManagerError_ShouldReturnRoomErrorMessage()
    {
        A.CallTo(() =>
                RoomManagerFake.CreateRoomAsync(userId, A<RoomPrivacy>._, A<CancellationToken>._, A<string>._,
                    maxPlayers))
            .Returns(OperationResult<Room>.Error.ExternalServiceError());
        
        var request = new CreateRoomRequest(userId, RoomPrivacy.Public, null, maxPlayers);
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<RoomDto>>("CreateRoom", request);
       
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ResultObj, Is.Null);
        });
        
        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ExternalServiceError));
        
        A.CallTo(() =>
            RoomManagerFake.CreateRoomAsync(userId, A<RoomPrivacy>._, A<CancellationToken>._, A<string>._,
                maxPlayers)).MustHaveHappenedOnceExactly();
        A.CallTo(() =>
            ConnectionServiceFake.AddConnection(A<string>._, A<Guid>._, A<Guid>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
    
        [Test]
    public async Task JoinRoom_CorrectRequest_ShouldReturnRoomAndAddConnection()
    {
        const string password = "12345";
        var room = new Room(Guid.NewGuid(), RoomPrivacy.Private, maxPlayers, password);
        
        A.CallTo(() => RoomManagerFake.JoinRoomAsync(room.Id, userId,A<CancellationToken>._, password))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => RoomManagerFake.GetRoomAsync(A<string>._, A<CancellationToken>._))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => ConnectionServiceFake.AddConnection(A<string>._, userId, room.Id, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        var request = new JoinRoomRequest(userId, inviteCode, password);
        
        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<RoomDto>>("JoinRoom", request);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj, Is.Not.Null);
            
        });
        Assert.That(result.ResultObj.Id, Is.EqualTo(room.Id));

        A.CallTo(() => RoomManagerFake.JoinRoomAsync(room.Id, userId,  A<CancellationToken>._, password))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => ConnectionServiceFake.AddConnection(A<string>._, userId, room.Id, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public async Task LeaveRoom_WhenSuccess_ShouldRemoveConnection()
    {
        var roomId = Guid.NewGuid();

        A.CallTo(() => RoomManagerFake.LeaveRoomAsync(roomId, userId, A<CancellationToken>._))
            .Returns(OperationResult.Ok());

        var request = new LeaveRoomRequest(userId, roomId);

        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult>("LeaveRoom", request);

        Assert.That(result.Success, Is.True);

        A.CallTo(() => RoomManagerFake.LeaveRoomAsync(roomId, userId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => ConnectionServiceFake.RemoveConnection(A<string>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public async Task FindQuickRoom_WhenFound_ShouldReturnRoomAndAddConnection()
    {
        var room = new Room(Guid.NewGuid(), RoomPrivacy.Public, maxPlayers, null);
        
        A.CallTo(() => RoomManagerFake.FindOrCreateQuickRoomAsync(userId, A<CancellationToken>._))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => RoomManagerFake.JoinRoomAsync(room.Id, userId, A<CancellationToken>._, null))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => ConnectionServiceFake.AddConnection(A<string>._, userId, room.Id, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        
        var request = new FindQuickRoomRequest(userId);

        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult<RoomDto>>("FindQuickRoom", request);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj, Is.Not.Null);
            
        });
        Assert.That(result.ResultObj.Id, Is.EqualTo(room.Id));
            
        A.CallTo(() => ConnectionServiceFake.AddConnection(A<string>._, userId, room.Id, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
        [Test]
    public async Task KickPlayer_WhenPlayerExists_ShouldRemoveTargetConnection()
    {
        var roomId = Guid.NewGuid();
        var targetPlayerId = Guid.NewGuid(); 
        var targetConnectionId = "test-conn-id-1";
        
        A.CallTo(() => RoomManagerFake.KickPlayerFromRoom(userId, roomId, targetPlayerId, A<CancellationToken>._))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => ConnectionServiceFake.GetConnectionIdByPlayer(targetPlayerId))
            .Returns(OperationResult<string>.Ok(targetConnectionId));
        
        A.CallTo(() => ConnectionServiceFake.RemoveConnection(targetConnectionId))
            .Returns(OperationResult.Ok());

        var request = new KickPlayerRequest(userId, roomId, targetPlayerId);

        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult>("KickPlayerFromRoom", request);

        Assert.That(result.Success, Is.True);
        
        A.CallTo(() => RoomManagerFake.KickPlayerFromRoom(userId, roomId, targetPlayerId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
            
        A.CallTo(() => ConnectionServiceFake.GetConnectionIdByPlayer(targetPlayerId))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => ConnectionServiceFake.RemoveConnection(targetConnectionId))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task KickPlayer_WhenRoomManagerError_ShouldReturnError()
    {
        var roomId = Guid.NewGuid();
        var targetPlayerId = Guid.NewGuid();
        
        A.CallTo(() => RoomManagerFake.KickPlayerFromRoom(userId, roomId, targetPlayerId, A<CancellationToken>._))
            .Returns(OperationResult.Error.Forbidden());

        var request = new KickPlayerRequest(userId, roomId, targetPlayerId);

        await HubConnection.StartAsync();
        var result = await HubConnection.InvokeAsync<OperationResult>("KickPlayerFromRoom", request);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Forbidden));
        });
        
        A.CallTo(() => ConnectionServiceFake.GetConnectionIdByPlayer(A<Guid>._))
            .MustNotHaveHappened();
    }
}