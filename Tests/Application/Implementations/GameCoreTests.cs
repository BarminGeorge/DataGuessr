using Application.Interfaces;
using Application.Notifications;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;
using FakeItEasy;
using Infrastructure.Interfaces;
using static NUnit.Framework.Assert;

namespace Tests.Application.Implementations;

[TestFixture]
public class GameCoreServiceTests
{
    private INotificationService notificationService;
    private IGameRepository gameRepository;
    private IQuestionService questionService;
    private IEvaluationService evaluationService;
    private IPlayerAnswerRepository answerRepository;
    
    private Game game;
    private Guid roomId;
    private CancellationToken ct;
    private int questionsCount;
    
    private GameCoreService service;

    [SetUp]
    public void Setup()
    {
        roomId = Guid.NewGuid();
        ct = CancellationToken.None;
        game = new Game(roomId, GameMode.DefaultMode, TimeSpan.Zero, 10);
        questionsCount = 2;

        notificationService = A.Fake<INotificationService>();
        gameRepository = A.Fake<IGameRepository>();
        questionService = A.Fake<IQuestionService>();
        evaluationService = A.Fake<IEvaluationService>();
        answerRepository = A.Fake<IPlayerAnswerRepository>();
        
        service = new GameCoreService(
            notificationService, 
            gameRepository, 
            questionService, 
            evaluationService, 
            answerRepository
        );
    }
    
    private List<Question> CreateQuestionsList()
    {
        var questions = Enumerable.Range(0, questionsCount)
            .Select<int, Question>(i => new Question(new Answer(DateTime.UtcNow.AddMinutes(1)), 
                $"Test Question{i}", $"FakeUrl{i}"))
            .ToList();
        return questions;
    }
    
    [Test]
    public async Task RunGameCycle_HappyPath_RunsThroughAllQuestionsAndFinishes()
    {
        var questions = CreateQuestionsList();

        A.CallTo(() => questionService.GetAllQuestionsAsync(game, ct))
            .Returns(OperationResult<IEnumerable<Question>>.Ok(questions));
        
        A.CallTo(() => answerRepository.LoadAnswersAsync(game.Id, A<Guid>._, ct))
            .Returns(OperationResult<Dictionary<Guid, Answer>>.Ok(new Dictionary<Guid, Answer>()));
        
        A.CallTo(() => gameRepository.SaveStatisticAsync(game.Id, A<Statistic>._, ct))
            .Returns(OperationResult.Ok());
        
        A.CallTo(() => evaluationService.CalculateScore(A<GameMode>._))
            .Returns((_, _) => new Score(10));

        A.CallTo(() => notificationService.NotifyGameRoomAsync(roomId, A<NewQuestionNotification>._))
            .Returns(OperationResult.Ok());
        A.CallTo(() => notificationService.NotifyGameRoomAsync(roomId, A<QuestionClosedNotification>._))
            .Returns(OperationResult.Ok());
        A.CallTo(() => notificationService.NotifyGameRoomAsync(roomId, A<StatisticNotification>._))
            .Returns(OperationResult.Ok());


        var result = await service.RunGameCycle(game, roomId, ct);
        
        Multiple(() =>
        {
            That(result.Success, Is.True);
            That(game.Status, Is.EqualTo(GameStatus.Finished));
        });

        A.CallTo(() => notificationService.NotifyGameRoomAsync(roomId, A<NewQuestionNotification>._))
            .MustHaveHappened(questionsCount, Times.Exactly);
        
        A.CallTo(() => notificationService.NotifyGameRoomAsync(roomId, A<QuestionClosedNotification>._))
            .MustHaveHappened(questionsCount, Times.Exactly);
        
        A.CallTo(() => notificationService.NotifyGameRoomAsync(roomId, A<StatisticNotification>._))
            .MustHaveHappened(questionsCount * 2, Times.Exactly);
    }

    [Test]
    public async Task RunGameCycle_WhenQuestionServiceFails_ReturnsError()
    {
        var errorMsg = "Service unavailable";
        A.CallTo(() => questionService.GetAllQuestionsAsync(game, ct))
            .Returns(OperationResult<IEnumerable<Question>>.Error.ServiceUnavailable(errorMsg));
        
        var result = await service.RunGameCycle(game, roomId, ct);
        
        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain(errorMsg));
        });
        
        A.CallTo(() => notificationService.NotifyGameRoomAsync(roomId, A<GameNotification>._))
            .MustNotHaveHappened();
    }

    [Test]
    public async Task RunGameCycle_WhenQuestionsAreNull_ReturnsSpecificError()
    {
        A.CallTo(() => questionService.GetAllQuestionsAsync(game, ct))
            .Returns(OperationResult<IEnumerable<Question>>.Ok(null));
        
        ThrowsAsync<InvalidCastException>(async () => 
            await service.RunGameCycle(game, roomId, ct));
        
    }

    [Test]
    public async Task RunGameCycle_WhenAnswerRepositoryFails_ReturnsErrorAndStops()
    {
        var questions = CreateQuestionsList();
        A.CallTo(() => questionService.GetAllQuestionsAsync(game, ct))
            .Returns(OperationResult<IEnumerable<Question>>.Ok(questions));
        
        A.CallTo(() => answerRepository.LoadAnswersAsync(game.Id, A<Guid>._, ct))
            .Returns(OperationResult<Dictionary<Guid, Answer>>.Error.ServiceUnavailable("Some db error"));
        
        var result = await service.RunGameCycle(game, roomId, ct);
        Multiple(() =>
        {
            That(result.Success, Is.False);
            That(result.ErrorMessage, Does.Contain("Some db error"));

            That(game.Status, Is.Not.EqualTo(GameStatus.Finished));
        });
    }
}