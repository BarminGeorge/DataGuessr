using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Notifications;
using Application.Result;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
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
    protected IUsersRepository UsersRepository;
    
    [SetUp]
    public void Setup()
    {
        RoomRepository = A.Fake<IRoomRepository>();
        NotificationService = A.Fake<INotificationService>();
        PlayerRepository = A.Fake<IPlayerRepository>();
        UsersRepository = A.Fake<IUsersRepository>();
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
        cancellationToken = CancellationToken.None;
        
        room = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        player = new Player(userId, roomId);
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