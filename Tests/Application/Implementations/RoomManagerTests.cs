using Application.Interfaces;
using Application.Notifications;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Infrastructure.Interfaces;

namespace Tests.Application.Implementations;


public class RoomManagerTests
{
    protected IRoomRepository RoomRepository;
    protected INotificationService NotificationService;
    protected IPlayerRepository PlayerRepository;
    protected IRoomManager RoomManager;
    protected IUserRepository UsersRepository;
    
    [SetUp]
    public void Setup()
    {
        RoomRepository = A.Fake<IRoomRepository>();
        NotificationService = A.Fake<INotificationService>();
        PlayerRepository = A.Fake<IPlayerRepository>();
        UsersRepository = A.Fake<IUserRepository>();
        RoomManager = new RoomManager(RoomRepository, NotificationService, PlayerRepository, UsersRepository);
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
                    room.Owner == userId 
                    && room.Privacy == privacy 
                    && room.MaxPlayers == maxPlayers), 
                cancellationToken))
            .Returns(result);

        var room = new Room(userId, privacy, maxPlayers);
        A.CallTo(() => RoomRepository.GetByIdAsync(A<Guid>._, A<CancellationToken>._))
            .Returns(OperationResult<Room>.Ok(room));

        var player = new Player(Guid.NewGuid(), userId, "");
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(A<Guid>._, A<CancellationToken>._))
            .Returns(OperationResult<Player>.Ok(player));

        var user = new User("login", "name", new Avatar("file", "mimetype"), "password");
        A.CallTo(() => UsersRepository.GetUsersByIds(A<IEnumerable<Guid>>._, cancellationToken))
            .Returns(OperationResult<IEnumerable<User>>.Ok([user]));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<NewPlayerNotification>._))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => UsersRepository.GetPlayerNameByIdAsync(A<Guid>._, A<CancellationToken>._))
            .Returns(OperationResult<string>.Ok("name"));
        
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, cancellationToken))
            .Returns(OperationResult.Ok());
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
            Assert.That(result.ResultObj!.Owner, Is.EqualTo(userId));
            Assert.That(result.ResultObj.Privacy, Is.EqualTo(privacy));
            Assert.That(result.ResultObj.MaxPlayers, Is.EqualTo(maxPlayers));
        });
    }
    
    [Test]
    public async Task CreateRoomAsync_WithError()
    {
        SetupRepositoryMock(OperationResult.Error.InternalError("err"));
        
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
    private User user;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        roomId = Guid.NewGuid();
        userId = Guid.NewGuid();
        var connectionId = Guid.NewGuid().ToString();
        cancellationToken = CancellationToken.None;

        var avatar = new Avatar("filename", "mimetype");
        user = new User("login", "name", avatar, "password");
        room = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        player = new Player(userId, roomId, connectionId);
    }

    private void SetupSuccessfulMocks()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));

        A.CallTo(() => UsersRepository.GetUsersByIds(A<IEnumerable<Guid>>._, cancellationToken))
            .Returns(OperationResult<IEnumerable<User>>.Ok([user]));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<NewPlayerNotification>._))
            .Returns(OperationResult.Ok());
        
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
            .Returns(OperationResult<Room>.Error.NotFound("Room not found"));

        var result = await RoomManager.JoinRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ResultObj, Is.Null);
            Assert.That(result.ErrorMessage, Is.EqualTo("Room not found"));
        });
    }

    [Test]
    public async Task JoinRoomAsync_WhenPlayerNotFound_ShouldReturnError()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Error.NotFound("Player not found"));

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
        
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<NewPlayerNotification>._))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .Returns(OperationResult.Error.InternalError("Update failed"));

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
        
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        
        A.CallTo(() => UsersRepository.GetPlayerNameByIdAsync(userId, cancellationToken))
            .Returns(OperationResult<string>.Ok("name"));
        
        A.CallTo(() => UsersRepository.GetUsersByIds(A<IEnumerable<Guid>>._, cancellationToken))
            .Returns(OperationResult<IEnumerable<User>>.Ok([user]));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<NewPlayerNotification>._))
            .Returns(OperationResult.Ok());
        
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
            .Returns(OperationResult<Room>.Error.NotFound(errorMessage));
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(errorMessage));
        });

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(A<Guid>._, A<CancellationToken>._))
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
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Error.NotFound(errorMessage));
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(errorMessage));
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
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
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .Returns(OperationResult.Error.InternalError(notificationError));
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(notificationError));
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._)).MustHaveHappened();
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Test]
    public async Task LeaveRoomAsync_WhenSuccessful_RemovesPlayerAndSendsNotification()
    {
        room.AddPlayer(player);

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .Returns(OperationResult.Ok());
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .Returns(OperationResult.Ok());
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True); 
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken)).MustHaveHappenedOnceExactly();
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(
            roomId, 
            A<PlayerLeavedNotification>._))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task LeaveRoomAsync_WhenRepositoryUpdateFails_ReturnsError()
    {
        const string updateError = "Update failed";
        room.AddPlayer(player);

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .Returns(OperationResult<Room>.Ok(room));
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .Returns(OperationResult.Ok());
        A.CallTo(() => RoomRepository.UpdateAsync(room, cancellationToken))
            .Returns(OperationResult.Error.InternalError(updateError));
        
        var result = await RoomManager.LeaveRoomAsync(roomId, userId, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(updateError));
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
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
        A.CallTo(() => PlayerRepository.GetPlayerByUserIdAsync(userId, cancellationToken))
            .Returns(OperationResult<Player>.Ok(player));
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<PlayerLeavedNotification>._))
            .ReturnsNextFromSequence(
                OperationResult.Error.InternalError("First fail"),
                OperationResult.Error.InternalError("Second fail"),
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
