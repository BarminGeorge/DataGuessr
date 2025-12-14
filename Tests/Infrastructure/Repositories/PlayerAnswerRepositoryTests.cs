using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;
using Infrastructure.PostgreSQL;
using Infrastructure.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests.Infrastructure.Repositories;

[TestFixture]
public class PlayerAnswerRepositoryTests
{
    private PlayerAnswerRepositoryTestContext context;
    private PlayerAnswerRepository playerAnswerRepository;
    private CancellationToken ct;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PlayerAnswerRepositoryTestContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new PlayerAnswerRepositoryTestContext(options);
        playerAnswerRepository = new PlayerAnswerRepository(context);
        ct = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        context?.Dispose();
    }

    [Test]
    public async Task SaveAnswerAsync_WithBoolAnswer_ShouldSaveAnswer()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        Answer answer = new BoolAnswer(true);

        var result = await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId, answer, ct);

        Assert.That(result.Success, Is.True);
        var savedAnswer = await context.PlayerAnswers
            .FirstOrDefaultAsync(pa => pa.GameId == gameId && pa.QuestionId == questionId && pa.PlayerId == playerId, ct);
        Assert.That(savedAnswer, Is.Not.Null);
        Assert.That(savedAnswer.Answer, Is.InstanceOf<BoolAnswer>());
        Assert.That(((BoolAnswer)savedAnswer.Answer).Value, Is.EqualTo(true));
    }

    [Test]
    public async Task SaveAnswerAsync_WithDateTimeAnswer_ShouldSaveAnswer()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;
        Answer answer = new DateTimeAnswer(dateTime);

        var result = await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId, answer, ct);

        Assert.That(result.Success, Is.True);
        var savedAnswer = await context.PlayerAnswers
            .FirstOrDefaultAsync(pa => pa.GameId == gameId && pa.QuestionId == questionId && pa.PlayerId == playerId, ct);
        Assert.That(savedAnswer, Is.Not.Null);
        Assert.That(savedAnswer.Answer, Is.InstanceOf<DateTimeAnswer>());
        Assert.That(((DateTimeAnswer)savedAnswer.Answer).Value, Is.EqualTo(dateTime));
    }

    [Test]
    public async Task SaveAnswerAsync_WithEmptyGameId_ShouldReturnValidationError()
    {
        var questionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        Answer answer = new BoolAnswer(false);

        var result = await playerAnswerRepository.SaveAnswerAsync(Guid.Empty, questionId, playerId, answer, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("GameId не может быть пустым GUID"));
    }

    [Test]
    public async Task SaveAnswerAsync_WithEmptyQuestionId_ShouldReturnValidationError()
    {
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        Answer answer = new BoolAnswer(false);

        var result = await playerAnswerRepository.SaveAnswerAsync(gameId, Guid.Empty, playerId, answer, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("QuestionId не может быть пустым GUID"));
    }

    [Test]
    public async Task SaveAnswerAsync_WithEmptyPlayerId_ShouldReturnValidationError()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        Answer answer = new BoolAnswer(false);

        var result = await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, Guid.Empty, answer, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("PlayerId не может быть пустым GUID"));
    }

    [Test]
    public async Task SaveAnswerAsync_WithNullAnswer_ShouldReturnValidationError()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();

        var result = await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId, null, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("Ответ не может быть null"));
    }

    [Test]
    public async Task SaveAnswerAsync_WithMultipleAnswers_ShouldSaveAll()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();
        Answer answer1 = new BoolAnswer(true);
        Answer answer2 = new BoolAnswer(false);

        var result1 = await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId1, answer1, ct);
        var result2 = await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId2, answer2, ct);

        Assert.That(result1.Success, Is.True);
        Assert.That(result2.Success, Is.True);

        var answers = await context.PlayerAnswers
            .Where(pa => pa.GameId == gameId && pa.QuestionId == questionId)
            .ToListAsync(ct);
        Assert.That(answers.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task LoadAnswersAsync_WithBoolAnswers_ShouldLoadAll()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();
        Answer answer1 = new BoolAnswer(true);
        Answer answer2 = new BoolAnswer(false);

        await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId1, answer1, ct);
        await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId2, answer2, ct);

        var result = await playerAnswerRepository.LoadAnswersAsync(gameId, questionId, ct);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ResultObj.Count, Is.EqualTo(2));
        Assert.That(result.ResultObj[playerId1], Is.InstanceOf<BoolAnswer>());
        Assert.That(result.ResultObj[playerId2], Is.InstanceOf<BoolAnswer>());
        Assert.That(((BoolAnswer)result.ResultObj[playerId1]).Value, Is.EqualTo(true));
        Assert.That(((BoolAnswer)result.ResultObj[playerId2]).Value, Is.EqualTo(false));
    }

    [Test]
    public async Task LoadAnswersAsync_WithDateTimeAnswers_ShouldLoadAll()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();
        var dateTime1 = DateTime.UtcNow;
        var dateTime2 = DateTime.UtcNow.AddHours(1);
        Answer answer1 = new DateTimeAnswer(dateTime1);
        Answer answer2 = new DateTimeAnswer(dateTime2);

        await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId1, answer1, ct);
        await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId2, answer2, ct);

        var result = await playerAnswerRepository.LoadAnswersAsync(gameId, questionId, ct);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ResultObj.Count, Is.EqualTo(2));
        Assert.That(result.ResultObj[playerId1], Is.InstanceOf<DateTimeAnswer>());
        Assert.That(((DateTimeAnswer)result.ResultObj[playerId1]).Value, Is.EqualTo(dateTime1));
        Assert.That(((DateTimeAnswer)result.ResultObj[playerId2]).Value, Is.EqualTo(dateTime2));
    }

    [Test]
    public async Task LoadAnswersAsync_WithEmptyGameId_ShouldReturnValidationError()
    {
        var questionId = Guid.NewGuid();

        var result = await playerAnswerRepository.LoadAnswersAsync(Guid.Empty, questionId, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("GameId не может быть пустым GUID"));
    }

    [Test]
    public async Task LoadAnswersAsync_WithEmptyQuestionId_ShouldReturnValidationError()
    {
        var gameId = Guid.NewGuid();

        var result = await playerAnswerRepository.LoadAnswersAsync(gameId, Guid.Empty, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("QuestionId не может быть пустым GUID"));
    }

    [Test]
    public async Task LoadAnswersAsync_WithNoAnswers_ShouldReturnEmptyDictionary()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        var result = await playerAnswerRepository.LoadAnswersAsync(gameId, questionId, ct);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ResultObj.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task LoadAnswersAsync_WithMixedAnswerTypes_ShouldLoadAll()
    {
        var gameId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();
        Answer answer1 = new BoolAnswer(true);
        Answer answer2 = new DateTimeAnswer(DateTime.UtcNow);

        await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId1, answer1, ct);
        await playerAnswerRepository.SaveAnswerAsync(gameId, questionId, playerId2, answer2, ct);

        var result = await playerAnswerRepository.LoadAnswersAsync(gameId, questionId, ct);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ResultObj.Count, Is.EqualTo(2));
        Assert.That(result.ResultObj[playerId1], Is.InstanceOf<BoolAnswer>());
        Assert.That(result.ResultObj[playerId2], Is.InstanceOf<DateTimeAnswer>());
    }
}
