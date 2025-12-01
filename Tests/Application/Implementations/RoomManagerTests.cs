using Application.Interfaces;
using Application.Notifications;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Tests.Application.Implementations;

[TestFixture]
public class RoomManagerTests
{
    protected IRoomRepository RoomRepository;
    protected INotificationService NotificationService;
    protected IPlayerRepository PlayerRepository;
    protected ILogger<RoomManager> Logger;
    protected IRoomManager RoomManager;
    protected IUserRepository UsersRepository;
    
    [SetUp]
    public void Setup()
    {
        RoomRepository = A.Fake<IRoomRepository>();
        NotificationService = A.Fake<INotificationService>();
        PlayerRepository = A.Fake<IPlayerRepository>();
        UsersRepository = A.Fake<IUserRepository>();
        Logger = A.Fake<ILogger<RoomManager>>();
        RoomManager = new RoomManager(RoomRepository, Logger, NotificationService, PlayerRepository, UsersRepository);
    }
}

[TestFixture]
public class CreateRoomTests : RoomManagerTests
{
    private Guid userId;
    private RoomPrivacy privacy;
    private int maxPlayers;
    private CancellationToken cancellationToken;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        userId = Guid.NewGuid();
        privacy = RoomPrivacy.Public;
        maxPlayers = 10;
        cancellationToken = CancellationToken.None;
    }

    private void SetupRepositoryMock(OperationResult result)
    {
        A.CallTo(() => RoomRepository.AddAsync(A<Room>.That.Matches(room =>
                    room.Owner == userId &&
                    room.Privacy == privacy &&
                    room.MaxPlayers == maxPlayers), 
                cancellationToken))
            .Returns(result);
    }

    [Test]
    public async Task CreateRoomAsync_WithValidParameters_ShouldCreateRoom()
    {
        SetupRepositoryMock(OperationResult.Ok());
        
        var result = await RoomManager.CreateRoomAsync(userId, privacy, cancellationToken, maxPlayers: maxPlayers);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj, Is.Not.Null);
            Assert.That(result.ResultObj.Owner, Is.EqualTo(userId));
            Assert.That(result.ResultObj.Privacy, Is.EqualTo(privacy));
            Assert.That(result.ResultObj.MaxPlayers, Is.EqualTo(maxPlayers));
        });
    }
    
    [Test]
    public async Task CreateRoomAsync_WithError()
    {
        SetupRepositoryMock(OperationResult.Error("err"));
        
        var result = await RoomManager.CreateRoomAsync(userId, privacy, cancellationToken, maxPlayers: maxPlayers);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ResultObj, Is.Null);
        });
    }
}

[TestFixture]
public class JoinRoomTests : RoomManagerTests
{
    private Guid roomId;
    private Guid userId;
    private Room room;
    private Player player;
    private CancellationToken cancellationToken;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        roomId = Guid.NewGuid();
        userId = Guid.NewGuid();
        var connectionId = Guid.NewGuid().ToString();
        cancellationToken = CancellationToken.None;
        
        room = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        player = new Player(userId, roomId, connectionId);
    }

    private void SetupSuccessfulMocks()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<NewPlayerNotification>
                .That.Matches(notification => 
                    notification.PlayerName == "name"
                    && notification.PlayerId == player.Id))).Returns(OperationResult.Ok());
        
        A.CallTo(() => UsersRepository.GetPlayerNameByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<string>.Ok("name"));
        
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .Returns(OperationResult.Ok());
    }

    [Test]
    public async Task JoinRoomAsync_WithValidParameters_ShouldJoinRoom()
    {
        SetupSuccessfulMocks();

        var result = await RoomManager.JoinRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj, Is.Not.Null);
        });
    }

    [Test]
    public async Task JoinRoomAsync_WhenRoomNotFound_ShouldReturnError()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Error("Room not found"));

        var result = await RoomManager.JoinRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ResultObj, Is.Null);
            Assert.That(result.ErrorMsg, Is.EqualTo("Room not found"));
        });
    }

    [Test]
    public async Task JoinRoomAsync_WhenPlayerNotFound_ShouldReturnError()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Error("Player not found"));

        var result = await RoomManager.JoinRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ResultObj, Is.Null);
        });
    }

    [Test]
    public async Task JoinRoomAsync_WhenUpdateFails_ShouldReturnError()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<NewPlayerNotification>
            .That.Matches(notification => 
                notification.PlayerName == "name"
                && notification.PlayerId == player.Id))).Returns(OperationResult.Ok());
        
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .Returns(OperationResult.Error("Update failed"));

        var result = await RoomManager.JoinRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ResultObj, Is.Null);
        });
    }

    [Test]
    public async Task JoinRoomAsync_WithPassword_WhenRoomRequiresPassword_ShouldValidatePassword()
    {
        var password = "test123";
        var privateRoom = new Room(Guid.NewGuid(), RoomPrivacy.Private, 10, password);
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(privateRoom));
        
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        
        A.CallTo(() => UsersRepository.GetPlayerNameByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<string>.Ok("name"));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<NewPlayerNotification>
            .That.Matches(notification => 
                notification.PlayerName == "name"
                && notification.PlayerId == player.Id))).Returns(OperationResult.Ok());
        
        A.CallTo(() => RoomRepository.UpdateAsync(privateRoom, cancellationToken))
            .Returns(OperationResult.Ok());

        var result = await RoomManager.JoinRoomAsync(roomId, userId, cancellationToken, password);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj, Is.Not.Null);
        });
    }

    [Test]
    public async Task JoinRoomAsync_WithWrongPassword_WhenRoomRequiresPassword_ShouldReturnError()
    {
        var correctPassword = "test123";
        var wrongPassword = "wrong";
        var privateRoom = new Room(Guid.NewGuid(), RoomPrivacy.Private, 10, correctPassword);
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(privateRoom));

        var result = await RoomManager.JoinRoomAsync(roomId, userId, cancellationToken, wrongPassword);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ResultObj, Is.Null);
        });
    }
}

public class LeaveRoomTests : RoomManagerTests
{
    private Guid roomId;
    private Guid userId;
    private Room room;
    private Player player;
    private CancellationToken cancellationToken;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        roomId = Guid.NewGuid();
        userId = Guid.NewGuid();
        var connectionId = Guid.NewGuid().ToString();
        cancellationToken = CancellationToken.None;

        room = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        player = new Player(userId, roomId, connectionId);
    }
    
    [Test]
    public async Task LeaveRoomAsync_WhenRoomNotFound_ReturnsError()
    { 
        var errorMessage = "Room not found";

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Error(errorMessage));
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Is.EqualTo(errorMessage));
        });

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(A<Guid>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<PlayerLeavedNotification>._))
            .MustNotHaveHappened();
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Test]
    public async Task LeaveRoomAsync_WhenPlayerNotFound_ReturnsError()
    {
        var errorMessage = "Player not found";

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Error(errorMessage));
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Is.EqualTo(errorMessage));
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<PlayerLeavedNotification>._))
            .MustNotHaveHappened();
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Test]
    public async Task LeaveRoomAsync_WhenNotificationFails_ReturnsError()
    {
        var notificationError = "Notification failed";

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .Returns(OperationResult.Error(notificationError));
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Is.EqualTo(notificationError));
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._)).MustHaveHappened();
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Test]
    public async Task LeaveRoomAsync_WhenSuccessful_RemovesPlayerAndSendsNotification()
    {
        room.AddPlayer(player);

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .Returns(OperationResult.Ok());
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .Returns(OperationResult.Ok());
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True); 
            Assert.That(room.Players.Contains(player), Is.False);
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken)).MustHaveHappenedOnceExactly();
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(
            roomId, 
            A<PlayerLeavedNotification>.That.Matches(n => 
                n.PlayerId == userId && n.OwnerId == room.Owner)))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task LeaveRoomAsync_WhenRepositoryUpdateFails_ReturnsError()
    {
        var updateError = "Update failed";
        room.AddPlayer(player);

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .Returns(OperationResult.Ok());
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .Returns(OperationResult.Error(updateError));
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Is.EqualTo(updateError));
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task LeaveRoomAsync_WithRetryLogic_RetriesOnNotificationFailure()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => PlayerRepository.GetPlayerByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .ReturnsNextFromSequence(
                OperationResult.Error("First fail"),
                OperationResult.Error("Second fail"),
                OperationResult.Ok()
            );
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .Returns(OperationResult.Ok());
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);

        Assert.That(result.Success, Is.True);
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .MustHaveHappened(3, Times.Exactly);
    }
}
