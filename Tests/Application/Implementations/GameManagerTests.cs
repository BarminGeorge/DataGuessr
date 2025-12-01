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
public class GameManagerTests
{
    protected IGameCoreService GameCoreService;
    protected IRoomRepository RoomRepository;
    protected INotificationService NotificationService;
    protected IQuestionService QuestionService;
    protected IGameRepository GameRepository;
    protected ILogger<GameManager> Logger;
    
    protected GameManager GameManager;
    
    [SetUp]
    public void SetUp()
    {
        GameCoreService = A.Fake<IGameCoreService>();
        RoomRepository = A.Fake<IRoomRepository>();
        NotificationService = A.Fake<INotificationService>();
        QuestionService = A.Fake<IQuestionService>();
        GameRepository = A.Fake<IGameRepository>();
        Logger = A.Fake<ILogger<GameManager>>();

        GameManager = new GameManager(GameCoreService, RoomRepository, NotificationService, QuestionService, GameRepository, Logger);
    }
}

[TestFixture]
public class StartNewGameTests : GameManagerTests
{
    private Guid roomId;
    private Guid userId;
    private CancellationToken ct;
    private Room validRoom;
    private Game existingGame;
    
    [SetUp]
    public void Setup()
    {
        SetUp();
        roomId = Guid.NewGuid();
        userId = Guid.NewGuid();
        ct = CancellationToken.None;
        
        existingGame = new Game(roomId, GameMode.DefaultMode, TimeSpan.FromMilliseconds(100), 5);
        validRoom = new Room(userId, RoomPrivacy.Public, 10);
    }
    
    [Test]
    public async Task StartNewGameAsync_WhenOwnerAndEnoughPlayersAndRoomAndGameExist_StartsGameInBackground()
    {
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        A.CallTo(() => RoomRepository.GetCurrentGameAsync(roomId, ct))
            .Returns(OperationResult<Game>.Ok(existingGame));
        
        var result = await GameManager.StartNewGameAsync(roomId, userId, ct);
        
        Assert.That(result.Success, Is.True);
        
        await Task.Delay(10);
        
        A.CallTo(() => GameCoreService.RunGameCycle(ct))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public async Task StartNewGameAsync_WhenUserIsNotOwner_ReturnsErrorAndDoesNotStartGame()
    {
        var notOwnerRoom = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(notOwnerRoom));
        
        var result = await GameManager.StartNewGameAsync(roomId, userId, ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Is.Not.Null);
            Assert.That(result.ErrorMsg, Is.Not.Empty);
            Assert.That(result.ErrorMsg, Does.Contain("You are not owner"));
        });
        
        A.CallTo(() => RoomRepository.GetCurrentGameAsync(A<Guid>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => GameCoreService.RunGameCycle(A<CancellationToken>._))
            .MustNotHaveHappened();
    }
    
    [Test]
    public async Task StartNewGameAsync_WhenNotEnoughPlayers_ReturnsError()
    {
        var singlePlayerRoom = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(singlePlayerRoom));
        
        var result = await GameManager.StartNewGameAsync(roomId, userId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Is.Not.Null);
            Assert.That(result.ErrorMsg, Does.Contain("Can't start new game"));
        });
        
        A.CallTo(() => RoomRepository.GetCurrentGameAsync(A<Guid>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
    
    [Test]
    public async Task StartNewGameAsync_WhenRoomNotFoundInCanStartCheck_ReturnsError()
    {
        const string errorMessage = "Room not found in database";
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Error(errorMessage));
        
        var result = await GameManager.StartNewGameAsync(roomId, userId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Is.Not.Null);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(A<Guid>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public async Task StartNewGameAsync_WhenRoomNotFoundAfterCanStartCheck_ReturnsError()
    {
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .ReturnsNextFromSequence(
                OperationResult<Room>.Ok(validRoom),
                OperationResult<Room>.Error("Room not found"));
        
        var result = await GameManager.StartNewGameAsync(roomId, userId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Is.Not.Null);
            Assert.That(result.ErrorMsg, Does.Contain("Room not found"));
        });
        
        A.CallTo(() => RoomRepository.GetCurrentGameAsync(A<Guid>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
    
    [Test]
    public async Task StartNewGameAsync_WhenCurrentGameNotFound_ReturnsError()
    {
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        const string gameErrorMessage = "Current game not found";
        A.CallTo(() => RoomRepository.GetCurrentGameAsync(roomId, ct))
            .Returns(Task.FromResult(OperationResult<Game>.Error(gameErrorMessage)));
        
        var result = await GameManager.StartNewGameAsync(roomId, userId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(gameErrorMessage));
        });
        
        A.CallTo(() => GameCoreService.RunGameCycle(A<CancellationToken>._))
            .MustNotHaveHappened();
    }
    
    [Test]
    public async Task StartNewGameAsync_WhenGameCoreServiceThrowsException_HandlesExceptionInBackground()
    {
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        A.CallTo(() => RoomRepository.GetCurrentGameAsync(roomId, ct))
            .Returns(OperationResult<Game>.Ok(existingGame));
        
        var exception = new Exception("Game core error");
        A.CallTo(() => GameCoreService.RunGameCycle(ct))
            .ThrowsAsync(exception);
        
        var result = await GameManager.StartNewGameAsync(roomId, userId, ct);
        
        Assert.That(result.Success, Is.True);
    }
    
    [Test]
    public async Task StartNewGameAsync_ReturnsImmediatelyWithoutWaitingForGameCycle()
    {
        var runGameCycleCalled = false;
        var completionSource = new TaskCompletionSource<bool>();
        
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        validRoom.AddPlayer(new Player(Guid.NewGuid(), roomId, ""));
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        A.CallTo(() => RoomRepository.GetCurrentGameAsync(roomId, ct))
            .Returns(OperationResult<Game>.Ok(existingGame));
        
        A.CallTo(() => GameCoreService.RunGameCycle(ct))
            .Invokes(async () => 
            {
                runGameCycleCalled = true;
                await completionSource.Task;
            });
        
        var startTime = DateTime.UtcNow;
        var result = await GameManager.StartNewGameAsync(roomId, userId, ct);
        var endTime = DateTime.UtcNow;
        var executionTime = endTime - startTime;

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(executionTime, Is.LessThan(TimeSpan.FromMilliseconds(100)));
        });
        
        completionSource.SetResult(true);
    }
}

[TestFixture]
public class CreateNewGameTests : GameManagerTests
{
    private Guid roomId;
    private Guid userId;
    private GameMode gameMode;
    private int countQuestions;
    private TimeSpan questionDuration;
    private CancellationToken ct;
    private IEnumerable<Question> testQuestions;
    private Room validRoom;
    
    [SetUp]
    public void Setup()
    {
        SetUp();
        roomId = Guid.NewGuid();
        userId = Guid.NewGuid();
        gameMode = GameMode.DefaultMode;
        countQuestions = 5;
        questionDuration = TimeSpan.FromSeconds(30);
        ct = CancellationToken.None;

        testQuestions = new List<Question>();

        validRoom = new Room(userId, RoomPrivacy.Public, 10);
    }
    
    [Test]
    public async Task CreateNewGameAsync_WhenValidParametersAndNoQuestions_CreatesGameSuccessfully()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        A.CallTo(() => GameRepository.AddGameAsync(A<Game>.That.Matches(g => 
            g.RoomId == roomId && 
            g.Mode == gameMode && 
            g.QuestionDuration == questionDuration), ct))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, ct))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(
            roomId, 
            A<NewGameNotification>.That.Matches(n => n.Game != null)))
            .Returns(OperationResult.Ok());
        
        var result = await GameManager.CreateNewGameAsync(roomId, userId, gameMode, countQuestions, questionDuration, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMsg, Is.Not.Null);
        });
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => GameRepository.AddGameAsync(A<Game>._, ct))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, ct))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<NewGameNotification>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public async Task CreateNewGameAsync_WhenValidParametersWithQuestions_CreatesGameWithQuestions()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        A.CallTo(() => GameRepository.AddGameAsync(A<Game>.That.Matches(g => 
            g.Questions.Count == testQuestions.Count()), ct))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, ct))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<NewGameNotification>._))
            .Returns(OperationResult.Ok());
        
        var result = await GameManager.CreateNewGameAsync(roomId, userId, gameMode, countQuestions, questionDuration, ct, testQuestions);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMsg, Is.Not.Null);
        });
    }
    
    [Test]
    public async Task CreateNewGameAsync_WhenRoomNotFound_ReturnsError()
    {
        const string errorMessage = "Room not found";
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Error(errorMessage));
        
        var result = await GameManager.CreateNewGameAsync(roomId, userId, gameMode, countQuestions, questionDuration, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });
        
        A.CallTo(() => GameRepository.AddGameAsync(A<Game>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<NewGameNotification>._))
            .MustNotHaveHappened();
    }
    
    [Test]
    public async Task CreateNewGameAsync_WhenUserIsNotOwner_ReturnsError()
    {
        var notOwnerRoom = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(notOwnerRoom));
        
        var result = await GameManager.CreateNewGameAsync(roomId, userId, gameMode, countQuestions, questionDuration, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain("Can't create new game, you are not the owner"));
        });
        
        A.CallTo(() => GameRepository.AddGameAsync(A<Game>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
    
    [Test]
    public async Task CreateNewGameAsync_WhenAddGameFails_ReturnsError()
    {
        const string errorMessage = "Failed to add game";
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        A.CallTo(() => GameRepository.AddGameAsync(A<Game>._, ct))
            .Returns(OperationResult.Error(errorMessage));
        
        var result = await GameManager.CreateNewGameAsync(roomId, userId, gameMode, countQuestions, questionDuration, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });
        
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<NewGameNotification>._))
            .MustNotHaveHappened();
    }
    
    [Test]
    public async Task CreateNewGameAsync_WhenUpdateRoomFails_ReturnsError()
    {
        const string errorMessage = "Failed to update room";
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        A.CallTo(() => GameRepository.AddGameAsync(A<Game>._, ct))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, ct))
            .Returns(OperationResult.Error(errorMessage));
        
        var result = await GameManager.CreateNewGameAsync(roomId, userId, gameMode, countQuestions, questionDuration, ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<NewGameNotification>._))
            .MustNotHaveHappened();
    }
    
    [Test]
    public async Task CreateNewGameAsync_WhenNotificationFails_ReturnsError()
    {
        const string errorMessage = "Failed to send notification";
        
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));
        
        A.CallTo(() => GameRepository.AddGameAsync(A<Game>._, ct))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, ct))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<NewGameNotification>._))
            .Returns(OperationResult.Error(errorMessage));
        
        var result = await GameManager.CreateNewGameAsync(
            roomId, userId, gameMode, countQuestions, questionDuration, ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });
    }

    [Test]
    public async Task CreateNewGameAsync_WhenNotificationWithRetryFails_ReturnsError()
    {
        const string errorMessage = "Failed after retry";

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));

        A.CallTo(() => GameRepository.AddGameAsync(A<Game>._, ct))
            .Returns(OperationResult.Ok());

        A.CallTo(() => RoomRepository.UpdateAsync(A<Room>._, ct))
            .Returns(OperationResult.Ok());

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<NewGameNotification>._))
            .Returns(OperationResult.Error(errorMessage))
            .NumberOfTimes(3);

        var result = await GameManager.CreateNewGameAsync(
            roomId, userId, gameMode, countQuestions, questionDuration, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });
    }
}

[TestFixture]
public class FinishGameTests : GameManagerTests
{
    private Guid roomId;
    private Guid userId;
    private CancellationToken ct;
    private Room validRoom;

    [SetUp]
    public void Setup()
    {
        SetUp();
        roomId = Guid.NewGuid();
        userId = Guid.NewGuid();
        ct = CancellationToken.None;

        validRoom = new Room(userId, RoomPrivacy.Public, 10);
    }

    [Test]
    public async Task FinishGameAsync_WhenOwnerAndRoomExists_ReturnsSuccessWithRoom()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId,
                A<ReturnToRoomNotification>._))
            .Returns(OperationResult.Ok());

        var result = await GameManager.FinishGameAsync(userId, roomId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj.Id, Is.EqualTo(validRoom.Id));
        });

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<ReturnToRoomNotification>._))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task FinishGameAsync_WhenRoomNotFound_ReturnsError()
    {
        const string errorMessage = "Room not found";
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Error(errorMessage));

        var result = await GameManager.FinishGameAsync(userId, roomId, ct);
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<ReturnToRoomNotification>._))
            .MustNotHaveHappened();
    }

    [Test]
    public async Task FinishGameAsync_WhenUserIsNotOwner_ReturnsError()
    {
        var notOwnerUserId = Guid.NewGuid();
        var roomWithDifferentOwner = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(roomWithDifferentOwner));

        var result = await GameManager.FinishGameAsync(notOwnerUserId, roomId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain("You are not the owner"));
        });

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<ReturnToRoomNotification>._))
            .MustNotHaveHappened();
    }

    [Test]
    public async Task FinishGameAsync_WhenNotificationFails_ReturnsError()
    {
        const string errorMessage = "Failed to send notification";

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<ReturnToRoomNotification>._))
            .Returns(OperationResult.Error(errorMessage));

        var result = await GameManager.FinishGameAsync(userId, roomId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task FinishGameAsync_WhenNotificationWithRetryFails_ReturnsError()
    {
        const string errorMessage = "Failed after retry";

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<ReturnToRoomNotification>._))
            .Returns(OperationResult.Error(errorMessage))
            .NumberOfTimes(3);

        var result = await GameManager.FinishGameAsync(userId, roomId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMsg, Does.Contain(errorMessage));
        });

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<ReturnToRoomNotification>._))
            .MustHaveHappened();
    }

    [Test]
    public async Task FinishGameAsync_WhenNotificationSucceedsAfterRetry_ReturnsSuccess()
    {
        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(validRoom));

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(roomId, A<ReturnToRoomNotification>._))
            .ReturnsNextFromSequence(
                OperationResult.Error("First attempt failed"),
                OperationResult.Ok()
            );

        var result = await GameManager.FinishGameAsync(userId, roomId, ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj, Is.Not.Null);
        });

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<ReturnToRoomNotification>._))
            .MustHaveHappenedTwiceExactly();
    }

    [Test]
    public async Task FinishGameAsync_WhenCalledWithDifferentUserIds_ChecksOwnershipCorrectly()
    {
        var ownerId = Guid.NewGuid();
        var nonOwnerId = Guid.NewGuid();
        var room = new Room(ownerId, RoomPrivacy.Public, 10);

        A.CallTo(() => RoomRepository.GetByIdAsync(roomId, ct))
            .Returns(OperationResult<Room>.Ok(room));

        A.CallTo(() => NotificationService.NotifyGameRoomAsync(A<Guid>._, A<ReturnToRoomNotification>._))
            .Returns(OperationResult.Ok());

        var ownerResult = await GameManager.FinishGameAsync(ownerId, roomId, ct);

        Assert.That(ownerResult.Success, Is.True);

        var nonOwnerResult = await GameManager.FinishGameAsync(nonOwnerId, roomId, ct);
        Assert.Multiple(() =>
        {
            Assert.That(nonOwnerResult.Success, Is.False);
            Assert.That(nonOwnerResult.ErrorMsg, Does.Contain("You are not the owner"));
        });
    }
}