using NUnit.Framework;
using static NUnit.Framework.Assert;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.PostgreSQL;
using Infrastructure.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Tests.Infrastructure.Repositories;

[TestFixture]
public class PlayerRepositoryTests
{
    private AppDbContext dbContext;
    private PlayerRepository playerRepository;
    private CancellationToken ct;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        dbContext = new PlayerRepositoryTestContext(options);
        dbContext.Database.EnsureCreated();

        playerRepository = new PlayerRepository(dbContext);
        ct = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        if (dbContext != null)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }
    }

    private User CreateTestUser(string login = "testuser", string playerName = "TestPlayer")
    {
        var avatar = new Avatar("avatar.jpg", "image/jpeg");
        return new User(login, playerName, avatar, "hashedPassword123");
    }

    private Room CreateTestRoom(Guid ownerId, RoomPrivacy privacy = RoomPrivacy.Public, int maxPlayers = 4)
    {
        return new Room(ownerId, privacy, maxPlayers);
    }

    private Player CreateTestPlayer(Guid userId, Guid roomId, string connectionId = "conn123")
    {
        return new Player(userId, roomId, connectionId);
    }

    [Test]
    public async Task GetPlayerByIdAsync_HappyPath_ReturnsPlayer()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var player = CreateTestPlayer(user.Id, room.Id, "conn001");
        await dbContext.Players.AddAsync(player);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.GetPlayerByIdAsync(player.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj, Is.Not.Null);
            That(result.ResultObj.Id, Is.EqualTo(player.Id));
            That(result.ResultObj.UserId, Is.EqualTo(user.Id));
            That(result.ResultObj.RoomId, Is.EqualTo(room.Id));
            That(result.ResultObj.ConnectionId, Is.EqualTo("conn001"));
        });
    }

    [Test]
    public async Task GetPlayerByIdAsync_PlayerNotFound_ReturnsNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        var result = await playerRepository.GetPlayerByIdAsync(nonExistentId, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найден"));
        });
    }

    [Test]
    public async Task GetPlayerByIdAsync_EmptyId_ReturnsValidationError()
    {
        var result = await playerRepository.GetPlayerByIdAsync(Guid.Empty, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }

    [Test]
    public async Task GetPlayerByConnectionIdAsync_HappyPath_ReturnsPlayerIdAndRoomId()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var player = CreateTestPlayer(user.Id, room.Id, "conn_special_123");
        await dbContext.Players.AddAsync(player);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.GetPlayerByConnectionIdAsync("conn_special_123");

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj.playerId, Is.EqualTo(player.Id));
            That(result.ResultObj.roomId, Is.EqualTo(room.Id));
        });
    }

    [Test]
    public async Task GetPlayerByConnectionIdAsync_ConnectionNotFound_ReturnsNotFound()
    {
        var result = await playerRepository.GetPlayerByConnectionIdAsync("non_existent_conn");

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найден"));
        });
    }

    [Test]
    public async Task GetPlayerByConnectionIdAsync_EmptyConnectionId_ReturnsValidationError()
    {
        var result = await playerRepository.GetPlayerByConnectionIdAsync("");

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_HappyPath_CreatesPlayerSuccessfully()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("new_conn_123", user.Id, room.Id, ct);

        var createdPlayer = dbContext.Players.FirstOrDefault(p => p.ConnectionId == "new_conn_123");

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(createdPlayer, Is.Not.Null);
            That(createdPlayer.UserId, Is.EqualTo(user.Id));
            That(createdPlayer.RoomId, Is.EqualTo(room.Id));
            That(createdPlayer.ConnectionId, Is.EqualTo("new_conn_123"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_RoomNotFound_ReturnsNotFound()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("conn_123", user.Id, Guid.NewGuid(), ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("Комната"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_UserNotFound_ReturnsNotFound()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("conn_123", Guid.NewGuid(), room.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("Пользователь"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_RoomFull_ReturnsInvalidOperation()
    {
        var user1 = CreateTestUser("user1", "Player1");
        var user2 = CreateTestUser("user2", "Player2");
        var user3 = CreateTestUser("user3", "Player3");

        await dbContext.Users.AddRangeAsync(user1, user2, user3);

        var room = CreateTestRoom(user1.Id, maxPlayers: 2);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var player1 = CreateTestPlayer(user1.Id, room.Id, "conn1");
        var player2 = CreateTestPlayer(user2.Id, room.Id, "conn2");
        await dbContext.Players.AddRangeAsync(player1, player2);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("conn3", user3.Id, room.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("заполнена"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_UserAlreadyInRoom_ReturnsAlreadyExists()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var player = CreateTestPlayer(user.Id, room.Id, "conn_first");
        await dbContext.Players.AddAsync(player);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("conn_second", user.Id, room.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("уже в этой комнате"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_ConnectionIdAlreadyExists_ReturnsAlreadyExists()
    {
        var user1 = CreateTestUser("user1", "Player1");
        var user2 = CreateTestUser("user2", "Player2");
        await dbContext.Users.AddRangeAsync(user1, user2);

        var room = CreateTestRoom(user1.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var player1 = CreateTestPlayer(user1.Id, room.Id, "same_conn_id");
        await dbContext.Players.AddAsync(player1);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("same_conn_id", user2.Id, room.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("уже занят"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_EmptyConnectionId_ReturnsValidationError()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("", user.Id, room.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_EmptyUserId_ReturnsValidationError()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("conn_123", Guid.Empty, room.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }

    [Test]
    public async Task CreatePlayerAsync_EmptyRoomId_ReturnsValidationError()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.CreatePlayerAsync("conn_123", user.Id, Guid.Empty, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }

    [Test]
    public async Task RemovePlayerByConnectionAsync_HappyPath_RemovesPlayerSuccessfully()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var player = CreateTestPlayer(user.Id, room.Id, "conn_to_remove");
        await dbContext.Players.AddAsync(player);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.RemovePlayerByConnectionAsync("conn_to_remove");

        var removedPlayer = dbContext.Players.FirstOrDefault(p => p.ConnectionId == "conn_to_remove");

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(removedPlayer, Is.Null);
        });
    }

    [Test]
    public async Task RemovePlayerByConnectionAsync_PlayerNotFound_ReturnsNotFound()
    {
        var result = await playerRepository.RemovePlayerByConnectionAsync("non_existent_conn");

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найден"));
        });
    }

    [Test]
    public async Task RemovePlayerByConnectionAsync_EmptyConnectionId_ReturnsValidationError()
    {
        var result = await playerRepository.RemovePlayerByConnectionAsync("");

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }

    [Test]
    public async Task GetConnectionByPlayerAsync_HappyPath_ReturnsConnectionId()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var player = CreateTestPlayer(user.Id, room.Id, "special_conn_456");
        await dbContext.Players.AddAsync(player);
        await dbContext.SaveChangesAsync();

        var result = await playerRepository.GetConnectionByPlayerAsync(player.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj, Is.EqualTo("special_conn_456"));
        });
    }

    [Test]
    public async Task GetConnectionByPlayerAsync_PlayerNotFound_ReturnsNotFound()
    {
        var result = await playerRepository.GetConnectionByPlayerAsync(Guid.NewGuid(), ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найден"));
        });
    }

    [Test]
    public async Task GetConnectionByPlayerAsync_EmptyPlayerId_ReturnsValidationError()
    {
        var result = await playerRepository.GetConnectionByPlayerAsync(Guid.Empty, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }
}
