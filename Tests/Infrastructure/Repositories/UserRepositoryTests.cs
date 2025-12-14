using Domain.Common;
using Domain.Entities;
using Infrastructure.PostgreSQL;
using Infrastructure.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace Tests.Infrastructure;

[TestFixture]
public class UserRepositoryTests
{
    private AppDbContext dbContext;
    private UserRepository userRepository;
    private CancellationToken ct;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        dbContext = new TestAppDbContext(options);
        dbContext.Database.EnsureCreated();

        userRepository = new UserRepository(dbContext);
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

    private Avatar CreateTestAvatar()
    {
        return new Avatar("avatar.jpg", "image/jpeg");
    }

    [Test]
    public async Task GetUsersByIds_HappyPath_ReturnsUsersWithAvatars()
    {
        var user1 = CreateTestUser("user1", "Player1");
        var user2 = CreateTestUser("user2", "Player2");

        await dbContext.Users.AddAsync(user1);
        await dbContext.Users.AddAsync(user2);
        await dbContext.SaveChangesAsync();

        var userIds = new[] { user1.Id, user2.Id };

        var result = await userRepository.GetUsersByIds(userIds, ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            var users = result.ResultObj.ToList();
            That(users, Has.Count.EqualTo(2));
            That(users.First().Login, Is.EqualTo("user1"));
            That(users.Last().Login, Is.EqualTo("user2"));
        });
    }

    [Test]
    public async Task GetUsersByIds_WhenNullIds_ReturnsValidationError()
    {
        var result = await userRepository.GetUsersByIds(null, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("null"));
        });
    }

    [Test]
    public async Task GetUsersByIds_WhenEmptyIds_ReturnsValidationError()
    {
        var result = await userRepository.GetUsersByIds(new List<Guid>(), ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("пустым"));
        });
    }

    [Test]
    public async Task GetUsersByIds_WhenUsersNotFound_ReturnsNotFoundError()
    {
        var nonExistentIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

        var result = await userRepository.GetUsersByIds(nonExistentIds, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найдены"));
        });
    }

    [Test]
    public async Task GetUsersByIds_WhenPartialMatch_ReturnsOnlyFoundUsers()
    {
        var user1 = CreateTestUser("user1");
        await dbContext.Users.AddAsync(user1);
        await dbContext.SaveChangesAsync();

        var userIds = new[] { user1.Id, Guid.NewGuid() };

        var result = await userRepository.GetUsersByIds(userIds, ct);

        Multiple(() =>
        {
            var users = result.ResultObj.ToList();
            That(users, Has.Count.EqualTo(1));
            That(users.First().Login, Is.EqualTo("user1"));
        });
    }

    [Test]
    public async Task AddAsync_HappyPath_AddsUserSuccessfully()
    {
        var user = CreateTestUser();

        var result = await userRepository.AddAsync(user, ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(dbContext.Users.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task AddAsync_WhenUserNull_ReturnsValidationError()
    {
        var result = await userRepository.AddAsync(null, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("null"));
        });
    }

    [Test]
    public async Task AddAsync_WhenLoginEmpty_ReturnsValidationError()
    {
        var avatar = new Avatar("avatar.jpg", "image/jpeg");
        var user = new User("", "PlayerName", avatar, "password");

        var result = await userRepository.AddAsync(user, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("Логин"));
        });
    }

    [Test]
    public async Task AddAsync_WhenLoginAlreadyExists_ReturnsAlreadyExistsError()
    {
        var user1 = CreateTestUser("duplicate");
        var user2 = CreateTestUser("duplicate");

        await userRepository.AddAsync(user1, ct);

        var result = await userRepository.AddAsync(user2, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("уже существует"));
        });
    }

    [Test]
    public async Task GetByLoginAsync_HappyPath_ReturnsUserWithAvatar()
    {
        var user = CreateTestUser("testlogin");

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var result = await userRepository.GetByLoginAsync("testlogin", ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj.Login, Is.EqualTo("testlogin"));
            That(result.ResultObj.Avatar, Is.Not.Null);
        });
    }

    [Test]
    public async Task GetByLoginAsync_WhenLoginEmpty_ReturnsValidationError()
    {
        var result = await userRepository.GetByLoginAsync("", ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("пустым"));
        });
    }

    [Test]
    public async Task GetByLoginAsync_WhenUserNotFound_ReturnsNotFoundError()
    {
        var result = await userRepository.GetByLoginAsync("nonexistent", ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найден"));
        });
    }

    [Test]
    public async Task UpdateUserAsync_HappyPath_UpdatesUserSuccessfully()
    {
        var user = CreateTestUser();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var newAvatar = CreateTestAvatar();

        var result = await userRepository.UpdateUserAsync(user.Id, newAvatar, "NewPlayerName", ct);

        var updatedUser = dbContext.Users.FirstOrDefault(u => u.Id == user.Id);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(updatedUser.PlayerName, Is.EqualTo("NewPlayerName"));
            That(updatedUser.Avatar, Is.Not.Null);
        });
    }


    [Test]
    public async Task UpdateUserAsync_WhenUserIdEmpty_ReturnsValidationError()
    {
        var avatar = CreateTestAvatar();

        var result = await userRepository.UpdateUserAsync(Guid.Empty, avatar, "NewName", ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("пустым GUID"));
        });
    }

    [Test]
    public async Task UpdateUserAsync_WhenPlayerNameEmpty_ReturnsValidationError()
    {
        var userId = Guid.NewGuid();
        var avatar = CreateTestAvatar();

        var result = await userRepository.UpdateUserAsync(userId, avatar, "", ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("PlayerName"));
        });
    }

    [Test]
    public async Task UpdateUserAsync_WhenAvatarNull_ReturnsValidationError()
    {
        var userId = Guid.NewGuid();

        var result = await userRepository.UpdateUserAsync(userId, null, "NewName", ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("Avatar"));
        });
    }

    [Test]
    public async Task UpdateUserAsync_WhenUserNotFound_ReturnsNotFoundError()
    {
        var userId = Guid.NewGuid();
        var avatar = CreateTestAvatar();

        var result = await userRepository.UpdateUserAsync(userId, avatar, "NewName", ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найден"));
        });
    }

    [Test]
    public async Task GetPlayerNameByIdAsync_HappyPath_ReturnsPlayerName()
    {
        var user = CreateTestUser("login", "TestPlayerName");
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var result = await userRepository.GetPlayerNameByIdAsync(user.Id, ct);

        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(result.ResultObj, Is.EqualTo("TestPlayerName"));
        });
    }

    [Test]
    public async Task GetPlayerNameByIdAsync_WhenIdEmpty_ReturnsValidationError()
    {
        var result = await userRepository.GetPlayerNameByIdAsync(Guid.Empty, ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("пустым GUID"));
        });
    }

    [Test]
    public async Task GetPlayerNameByIdAsync_WhenUserNotFound_ReturnsNotFoundError()
    {
        var result = await userRepository.GetPlayerNameByIdAsync(Guid.NewGuid(), ct);

        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("не найден"));
        });
    }
}
