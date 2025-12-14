using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;
using Infrastructure.Interfaces;
using Infrastructure.PostgreSQL;
using Infrastructure.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests.Infrastructure.Repositories;

[TestFixture]
public class GameRepositoryTests
{
    private GameRepositoryTestContext context;
    private GameRepository gameRepository;
    private CancellationToken ct;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<GameRepositoryTestContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        context = new GameRepositoryTestContext(options);
        gameRepository = new GameRepository(context);
        ct = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        context?.Dispose();
    }

    [Test]
    public async Task SaveStatisticAsync_WithValidGameId_ShouldSaveStatistic()
    {
        var gameId = Guid.NewGuid();
        var game = new Game(
            Guid.NewGuid(),
            GameMode.DefaultMode,
            TimeSpan.FromSeconds(30),
            10
        );

        await context.Games.AddAsync(game, ct);
        await context.SaveChangesAsync(ct);

        var statistic = new Statistic();

        var result = await gameRepository.SaveStatisticAsync(game.Id, statistic, ct);

        Assert.That(result.Success, Is.True);
        var savedGame = await context.Games.FirstOrDefaultAsync(g => g.Id == game.Id, ct);
        Assert.That(savedGame.CurrentStatistic, Is.Not.Null);
    }

    [Test]
    public async Task SaveStatisticAsync_WithEmptyGameId_ShouldReturnValidationError()
    {
        var statistic = new Statistic();

        var result = await gameRepository.SaveStatisticAsync(Guid.Empty, statistic, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("GameId не может быть пустым GUID"));
    }

    [Test]
    public async Task SaveStatisticAsync_WithNullStatistic_ShouldReturnValidationError()
    {
        var gameId = Guid.NewGuid();

        var result = await gameRepository.SaveStatisticAsync(gameId, null, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("Статистика не может быть null"));
    }

    [Test]
    public async Task SaveStatisticAsync_WithNonExistentGameId_ShouldReturnNotFoundError()
    {
        var gameId = Guid.NewGuid();
        var statistic = new Statistic();

        var result = await gameRepository.SaveStatisticAsync(gameId, statistic, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("NotFound"));
        Assert.That(result.ErrorMessage, Contains.Substring($"Игра с ID '{gameId}' не найдена"));
    }

    [Test]
    public async Task SaveStatisticAsync_ShouldUpdateExistingStatistic()
    {
        var game = new Game(
            Guid.NewGuid(),
            GameMode.DefaultMode,
            TimeSpan.FromSeconds(30),
            10
        );

        await context.Games.AddAsync(game, ct);
        await context.SaveChangesAsync(ct);

        var firstStatistic = new Statistic();
        var secondStatistic = new Statistic();

        var result1 = await gameRepository.SaveStatisticAsync(game.Id, firstStatistic, ct);
        var result2 = await gameRepository.SaveStatisticAsync(game.Id, secondStatistic, ct);

        Assert.That(result1.Success, Is.True);
        Assert.That(result2.Success, Is.True);

        var savedGame = await context.Games.FirstOrDefaultAsync(g => g.Id == game.Id, ct);
        Assert.That(savedGame.CurrentStatistic, Is.Not.Null);
    }


    [Test]
    public async Task AddGameAsync_WithValidGame_ShouldAddGame()
    {
        var roomId = Guid.NewGuid();
        var game = new Game(
            roomId,
            GameMode.DefaultMode,
            TimeSpan.FromSeconds(30),
            10
        );

        var result = await gameRepository.AddGameAsync(game, ct);

        Assert.That(result.Success, Is.True);
        var savedGame = await context.Games.FirstOrDefaultAsync(g => g.Id == game.Id, ct);
        Assert.That(savedGame, Is.Not.Null);
        Assert.That(savedGame.RoomId, Is.EqualTo(roomId));
        Assert.That(savedGame.Status, Is.EqualTo(GameStatus.NotStarted));
    }

    [Test]
    public async Task AddGameAsync_WithNullGame_ShouldReturnValidationError()
    {
        var result = await gameRepository.AddGameAsync(null, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("Игра не может быть null"));
    }


    [Test]
    public async Task AddGameAsync_WithMultipleGames_ShouldAddAllGames()
    {
        var roomId = Guid.NewGuid();
        var game1 = new Game(
            roomId,
            GameMode.DefaultMode,
            TimeSpan.FromSeconds(30),
            10
        );

        var game2 = new Game(
            roomId,
            GameMode.BoolMode,
            TimeSpan.FromSeconds(15),
            5
        );

        var result1 = await gameRepository.AddGameAsync(game1, ct);
        var result2 = await gameRepository.AddGameAsync(game2, ct);

        Assert.That(result1.Success, Is.True);
        Assert.That(result2.Success, Is.True);

        var games = await context.Games.Where(g => g.RoomId == roomId).ToListAsync(ct);
        Assert.That(games.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task AddGameAsync_ShouldPreserveGameProperties()
    {
        var roomId = Guid.NewGuid();
        var mode = GameMode.DefaultMode;
        var questionCount = 15;
        var duration = TimeSpan.FromSeconds(45);

        var game = new Game(roomId, mode, duration, questionCount);

        var result = await gameRepository.AddGameAsync(game, ct);

        Assert.That(result.Success, Is.True);

        var savedGame = await context.Games.FirstOrDefaultAsync(g => g.Id == game.Id, ct);
        Assert.That(savedGame.Id, Is.EqualTo(game.Id));
        Assert.That(savedGame.RoomId, Is.EqualTo(roomId));
        Assert.That(savedGame.Mode, Is.EqualTo(mode));
        Assert.That(savedGame.Status, Is.EqualTo(GameStatus.NotStarted));
        Assert.That(savedGame.QuestionsCount, Is.EqualTo(questionCount));
        Assert.That(savedGame.QuestionDuration, Is.EqualTo(duration));
    }

}
