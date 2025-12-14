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
public class QuestionRepositoryTests
{
    private QuestionRepositoryTestContext context;
    private QuestionRepository questionRepository;
    private CancellationToken ct;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<QuestionRepositoryTestContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new QuestionRepositoryTestContext(options);
        questionRepository = new QuestionRepository(context);
        ct = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        context?.Dispose();
    }

    [Test]
    public async Task GetUniqQuestionsAsync_WithValidMode_ShouldReturnQuestions()
    {
        var question1 = new Question(new BoolAnswer(true), "What is 2+2?", GameMode.DefaultMode);
        var question2 = new Question(new BoolAnswer(false), "What is 3+3?", GameMode.DefaultMode);
        var question3 = new Question(new BoolAnswer(true), "What is 4+4?", GameMode.DefaultMode);

        await context.Questions.AddAsync(question1, ct);
        await context.Questions.AddAsync(question2, ct);
        await context.Questions.AddAsync(question3, ct);
        await context.SaveChangesAsync(ct);

        var result = await questionRepository.GetUniqQuestionsAsync(2, GameMode.DefaultMode, ct);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ResultObj.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetUniqQuestionsAsync_WithZeroCount_ShouldReturnValidationError()
    {
        var result = await questionRepository.GetUniqQuestionsAsync(0, GameMode.DefaultMode, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("Количество вопросов должно быть больше нуля"));
    }

    [Test]
    public async Task GetUniqQuestionsAsync_WithNegativeCount_ShouldReturnValidationError()
    {
        var result = await questionRepository.GetUniqQuestionsAsync(-5, GameMode.DefaultMode, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("Validation"));
        Assert.That(result.ErrorMessage, Contains.Substring("Количество вопросов должно быть больше нуля"));
    }

    [Test]
    public async Task GetUniqQuestionsAsync_WithNoQuestionsForMode_ShouldReturnNotFoundError()
    {
        var result = await questionRepository.GetUniqQuestionsAsync(5, GameMode.BoolMode, ct);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType.ToString(), Contains.Substring("NotFound"));
        Assert.That(result.ErrorMessage, Contains.Substring($"Вопросы для режима {GameMode.BoolMode} не найдены в базе данных"));
    }

    [Test]
    public async Task GetUniqQuestionsAsync_WithCountGreaterThanAvailable_ShouldReturnAllAvailable()
    {
        var question1 = new Question(new BoolAnswer(true), "What is 2+2?", GameMode.DefaultMode);
        var question2 = new Question(new BoolAnswer(false), "What is 3+3?", GameMode.DefaultMode);

        await context.Questions.AddAsync(question1, ct);
        await context.Questions.AddAsync(question2, ct);
        await context.SaveChangesAsync(ct);

        var result = await questionRepository.GetUniqQuestionsAsync(10, GameMode.DefaultMode, ct);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ResultObj.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetUniqQuestionsAsync_ShouldReturnRequestedCount()
    {
        var question1 = new Question(new BoolAnswer(true), "What is 2+2?", GameMode.DefaultMode);
        var question2 = new Question(new BoolAnswer(false), "What is 3+3?", GameMode.DefaultMode);
        var question3 = new Question(new BoolAnswer(true), "What is 4+4?", GameMode.DefaultMode);
        var question4 = new Question(new BoolAnswer(false), "What is 5+5?", GameMode.DefaultMode);

        await context.Questions.AddAsync(question1, ct);
        await context.Questions.AddAsync(question2, ct);
        await context.Questions.AddAsync(question3, ct);
        await context.Questions.AddAsync(question4, ct);
        await context.SaveChangesAsync(ct);

        var result = await questionRepository.GetUniqQuestionsAsync(3, GameMode.DefaultMode, ct);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ResultObj.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetUniqQuestionsAsync_ShouldReturnQuestionsForCorrectMode()
    {
        var question1 = new Question(new BoolAnswer(true), "What is 2+2?", GameMode.DefaultMode);
        var question2 = new Question(new BoolAnswer(false), "What is 3+3?", GameMode.BoolMode);
        var question3 = new Question(new BoolAnswer(true), "What is 4+4?", GameMode.DefaultMode);

        await context.Questions.AddAsync(question1, ct);
        await context.Questions.AddAsync(question2, ct);
        await context.Questions.AddAsync(question3, ct);
        await context.SaveChangesAsync(ct);

        var result = await questionRepository.GetUniqQuestionsAsync(5, GameMode.DefaultMode, ct);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ResultObj.All(q => q.Mode == GameMode.DefaultMode), Is.True);
    }

    [Test]
    public async Task GetUniqQuestionsAsync_ShouldReturnDifferentQuestionsOnMultipleCalls()
    {
        var question1 = new Question(new BoolAnswer(true), "What is 2+2?", GameMode.DefaultMode);
        var question2 = new Question(new BoolAnswer(false), "What is 3+3?", GameMode.DefaultMode);
        var question3 = new Question(new BoolAnswer(true), "What is 4+4?", GameMode.DefaultMode);

        await context.Questions.AddAsync(question1, ct);
        await context.Questions.AddAsync(question2, ct);
        await context.Questions.AddAsync(question3, ct);
        await context.SaveChangesAsync(ct);

        var result1 = await questionRepository.GetUniqQuestionsAsync(2, GameMode.DefaultMode, ct);
        var result2 = await questionRepository.GetUniqQuestionsAsync(2, GameMode.DefaultMode, ct);

        Assert.That(result1.Success, Is.True);
        Assert.That(result2.Success, Is.True);
        Assert.That(result1.ResultObj.Count(), Is.EqualTo(2));
        Assert.That(result2.ResultObj.Count(), Is.EqualTo(2));
    }
}
