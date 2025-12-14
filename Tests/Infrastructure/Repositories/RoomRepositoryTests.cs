using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;
using Infrastructure.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace Tests.Infrastructure.Repositories;

[TestFixture]
public class RoomRepositoryTests
{
    private RoomRepositoryTestContext dbContext;
    private RoomRepository roomRepository;
    private CancellationToken ct;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<RoomRepositoryTestContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        dbContext = new RoomRepositoryTestContext(options);

        roomRepository = new RoomRepository(dbContext);
        ct = CancellationToken.None;
    }



    [TearDown]
    public void TearDown()
    {
        if (dbContext != null)
        {
            dbContext.Dispose();
        }
    }

    private User CreateTestUser(string login = "testuser", string playerName = "TestPlayer")
    {
        var avatar = new Avatar("avatar.jpg", "image/jpeg");
        return new User(login, playerName, avatar, "hashedPassword123");
    }

    private Room CreateTestRoom(Guid ownerId, RoomPrivacy privacy = RoomPrivacy.Public, int maxPlayers = 4, string password = null)
    {
        return new Room(ownerId, privacy, maxPlayers, password);
    }

    private Player CreateTestPlayer(Guid userId, Guid roomId, string connectionId)
    {
        return new Player(userId, roomId, connectionId);
    }

    [Test]
    public async Task GetByIdAsync_HappyPath_ReturnsRoom()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var room = CreateTestRoom(user.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var result = await roomRepository.GetByIdAsyncForTesting(room.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj, Is.Not.Null);
            That(result.ResultObj.Id, Is.EqualTo(room.Id));
            That(result.ResultObj.Owner, Is.EqualTo(user.Id));
            That(result.ResultObj.Privacy, Is.EqualTo(RoomPrivacy.Public));
            That(result.ResultObj.MaxPlayers, Is.EqualTo(4));
        });
    }

    [Test]
    public async Task GetByIdAsync_RoomNotFound_ReturnsNotFound()
    {
        var result = await roomRepository.GetByIdAsyncForTesting(Guid.NewGuid(), ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найдена"));
        });
    }

    [Test]
    public async Task GetByIdAsync_EmptyId_ReturnsValidationError()
    {
        var result = await roomRepository.GetByIdAsync(Guid.Empty, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }

    [Test]
    public async Task GetByIdAsync_WithPlayers_ReturnsRoomWithPlayers()
    {
        var user1 = CreateTestUser("user1", "Player1");
        var user2 = CreateTestUser("user2", "Player2");
        await dbContext.Users.AddRangeAsync(user1, user2);
        await dbContext.SaveChangesAsync();

        var room = CreateTestRoom(user1.Id);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var player1 = CreateTestPlayer(user1.Id, room.Id, "conn1");
        var player2 = CreateTestPlayer(user2.Id, room.Id, "conn2");
        await dbContext.Players.AddRangeAsync(player1, player2);
        await dbContext.SaveChangesAsync();

        var result = await roomRepository.GetByIdAsyncForTesting(room.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj.Players.Count, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetWaitingPublicRoomsAsync_HappyPath_ReturnsAvailablePublicRooms()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var room1 = CreateTestRoom(user.Id, RoomPrivacy.Public);
        var room2 = CreateTestRoom(user.Id, RoomPrivacy.Public);
        var room3 = CreateTestRoom(user.Id, RoomPrivacy.Private);

        await dbContext.Rooms.AddRangeAsync(room1, room2, room3);
        await dbContext.SaveChangesAsync();

        var result = await roomRepository.GetWaitingPublicRoomsAsync(ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetWaitingPublicRoomsAsync_NoPublicRooms_ReturnsEmpty()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);

        var room = CreateTestRoom(user.Id, RoomPrivacy.Private);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var result = await roomRepository.GetWaitingPublicRoomsAsync(ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj.Count(), Is.EqualTo(0));
        });
    }

    [Test]
    public async Task GetWaitingPublicRoomsAsync_ExpiredRoomsExcluded_ReturnsOnlyValidRooms()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var validRoom = CreateTestRoom(user.Id, RoomPrivacy.Public);
        var expiredRoom = new Room(user.Id, RoomPrivacy.Public, 4, null, TimeSpan.FromHours(-1));

        await dbContext.Rooms.AddRangeAsync(validRoom, expiredRoom);
        await dbContext.SaveChangesAsync();

        var result = await roomRepository.GetWaitingPublicRoomsAsync(ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj.Count(), Is.EqualTo(1));
            That(result.ResultObj.First().Id, Is.EqualTo(validRoom.Id));
        });
    }

    [Test]
    public async Task AddAsync_HappyPath_CreatesRoomSuccessfully()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var room = CreateTestRoom(user.Id);
        var result = await roomRepository.AddAsync(room, ct);

        var createdRoom = dbContext.Rooms.FirstOrDefault(r => r.Id == room.Id);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(createdRoom, Is.Not.Null);
            That(createdRoom.Owner, Is.EqualTo(user.Id));
        });
    }

    [Test]
    public async Task AddAsync_NullRoom_ReturnsValidationError()
    {
        var result = await roomRepository.AddAsync(null, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть null"));
        });
    }

    [Test]
    public async Task AddAsync_PrivateRoomWithPassword_CreatesSuccessfully()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var room = CreateTestRoom(user.Id, RoomPrivacy.Private, 4, "password123");
        var result = await roomRepository.AddAsync(room, ct);

        var createdRoom = dbContext.Rooms.FirstOrDefault(r => r.Id == room.Id);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(createdRoom.Privacy, Is.EqualTo(RoomPrivacy.Private));
            That(createdRoom.Password, Is.EqualTo("password123"));
        });
    }

    [Test]
    public async Task UpdateAsync_HappyPath_UpdatesRoomSuccessfully()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var room = CreateTestRoom(user.Id, RoomPrivacy.Public);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.SaveChangesAsync();

        var result = await roomRepository.UpdateAsync(room, ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
        });
    }

    [Test]
    public async Task UpdateAsync_RoomNotFound_ReturnsNotFound()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var nonExistentRoom = CreateTestRoom(user.Id);
        var result = await roomRepository.UpdateAsync(nonExistentRoom, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найдена"));
        });
    }

    [Test]
    public async Task UpdateAsync_NullRoom_ReturnsValidationError()
    {
        var result = await roomRepository.UpdateAsync(null, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть null"));
        });
    }

    [Test]
    public async Task UpdateAsync_EmptyId_ReturnsValidationError()
    {
        var room = new Room(Guid.NewGuid(), RoomPrivacy.Public, 4);

        typeof(Room).GetProperty("Id")?.SetValue(room, Guid.Empty);

        var result = await roomRepository.UpdateAsync(room, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не может быть пустым"));
        });
    }

}
