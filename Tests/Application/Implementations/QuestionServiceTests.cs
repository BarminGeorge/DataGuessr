using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Infrastructure.Interfaces;

namespace Tests.Application.Implementations;

public class QuestionServiceTests
{
    private IQuestionRepository questionRepository;
    private IPlayerAnswerRepository answersRepository;
    private Guid roomId;
    private TimeSpan questionDuraction;
    private int questionsCount;
    private CancellationToken cancellationToken;
    
    private QuestionService questionService;
    
    [SetUp]
    public void Setup()
    {
        roomId = Guid.NewGuid();
        cancellationToken = CancellationToken.None;
        questionDuraction = TimeSpan.FromSeconds(30);
        questionsCount = 10;
        questionRepository = A.Fake<IQuestionRepository>();
        answersRepository = A.Fake<IPlayerAnswerRepository>();
        
        questionService = new QuestionService(questionRepository, answersRepository);
        A.CallTo(() => questionRepository.GetUniqQuestionsAsync(A<int>.That.Matches(x => x >= 0), GameMode.DefaultMode,
                A<CancellationToken>.Ignored))
            .Returns(OperationResult<IEnumerable<Question>>.Ok(A.CollectionOfFake<Question>(questionsCount)));
    }
    
    [Test]
    public async Task GetAllQuestionsAsync_WithGameContainingQuestion_ReturnsAllQuestionsFromGameField()
    {
        var gameWithQuestions = new Game(roomId, GameMode.DefaultMode, questionDuraction, questionsCount);
        var fakeQuestions = A.CollectionOfFake<Question>(questionsCount);
        gameWithQuestions.AddQuestions(fakeQuestions);
        
        var result = await questionService.GetAllQuestionsAsync(gameWithQuestions, cancellationToken);
        
        Assert.That(result.ResultObj, Is.EqualTo(gameWithQuestions.Questions));
        
        A.CallTo(() => questionRepository.GetUniqQuestionsAsync(A<int>._, GameMode.DefaultMode, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Test]
    public async Task GetAllQuestionsAsync_WithGameNotContainingQuestion_ReturnsAllQuestionsFromDatabase()
    {
        var gameWithoutQuestions = new Game(roomId, GameMode.DefaultMode, questionDuraction, questionsCount);
        
        var result = await questionService.GetAllQuestionsAsync(gameWithoutQuestions, cancellationToken);
        Assert.That(result.ResultObj, Is.Not.Null);
        Assert.That(result.ResultObj!.Count(), Is.EqualTo(questionsCount));
        
        A.CallTo(() => questionRepository.GetUniqQuestionsAsync(questionsCount, GameMode.DefaultMode, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}