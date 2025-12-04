using Application.Services;
using Domain.Enums;
using Domain.ValueTypes;

namespace Tests.Application.Implementations;

[TestFixture]
public class EvaluateServiceTests
{
    private EvaluateService evaluateService;
        
    [SetUp]
    public void Setup()
    {
        evaluateService = new EvaluateService();
    }

    [Test]
    public void CalculateScore_DefaultMode_ReturnsCorrectDelegate()
    {
        var mode = GameMode.DefaultMode;
        var answer = new DateTimeAnswer( new DateTime(2023, 1, 1) );
        var rightAnswer = new DateTimeAnswer(new DateTime(2023, 1, 10));
            
        var scoreCalculator = evaluateService.CalculateScore(mode);
        var score = scoreCalculator(answer, rightAnswer);
            
        Assert.IsNotNull(scoreCalculator);
        Assert.IsNotNull(score);
        Assert.That(score.score, Is.GreaterThan(0));
    }

    [Test]
    public void CalculateScore_DefaultMode_SameDate_ReturnsMaxScore()
    {
        var mode = GameMode.DefaultMode;
        var date = new DateTime(2023, 1, 1);
        var answer = new DateTimeAnswer(date);
        var rightAnswer =  new DateTimeAnswer(date);
            
        var scoreCalculator = evaluateService.CalculateScore(mode);
        var score = scoreCalculator(answer, rightAnswer);
            
        Assert.That(score.score, Is.EqualTo(2222));
    }

    [Test]
    public void CalculateScore_DefaultMode_OneDayDifference_ReturnsCorrectScore()
    {

        var mode = GameMode.DefaultMode;
        var answer = new DateTimeAnswer(new DateTime(2023, 1, 1) );
        var rightAnswer = new DateTimeAnswer(new DateTime(2023, 1, 2)); 
  
        var scoreCalculator = evaluateService.CalculateScore(mode);
        var score = scoreCalculator(answer, rightAnswer);
            
        var expected = (int)Math.Round(2222 * Math.Exp(-1 * 1.0 / 10000));
        Assert.That(score.score, Is.EqualTo(expected));
    }

    [Test]
    public void CalculateScore_DefaultMode_LargeDifference_ReturnsMinScore()
    {
      
        var mode = GameMode.DefaultMode;
        var answer = new DateTimeAnswer(new DateTime(2023, 1, 1));
        var rightAnswer = new DateTimeAnswer(new DateTime(234, 1, 1)); 


        var scoreCalculator = evaluateService.CalculateScore(mode);
        var score = scoreCalculator(answer, rightAnswer);


        Assert.That(score.score, Is.GreaterThanOrEqualTo(0));
        Assert.That(score.score, Is.LessThan(100));
    }

    [Test]
    public void CalculateScore_DefaultMode_NegativeDifference_ReturnsCorrectScore()
    {
  
        var mode = GameMode.DefaultMode;
        var answer = new DateTimeAnswer(new DateTime(2023, 1, 10));
        var rightAnswer =new DateTimeAnswer(new DateTime(2023, 1, 1)); 

     
        var scoreCalculator = evaluateService.CalculateScore(mode);
        var score = scoreCalculator(answer, rightAnswer);
            
        var expected = (int)Math.Round(2222 * Math.Exp(-1 * 9.0 / 10000));
        Assert.That(score.score, Is.EqualTo(expected));
    }

    [Test]
    public void CalculateScore_UnsupportedMode_ThrowsArgumentException()
    {
 
        var unsupportedMode = (GameMode)999;
    
        var ex = Assert.Throws<ArgumentException>(() => 
            evaluateService.CalculateScore(unsupportedMode));
            
        Assert.That(ex.Message, Does.Contain("не реализован"));
    }

    [Test]
    public void CalculateScore_DefaultMode_WithNullAnswer_ThrowsException()
    {
    
        var mode = GameMode.DefaultMode;

        var scoreCalculator = evaluateService.CalculateScore(mode);
            
        Assert.Throws<NullReferenceException>(() => 
            scoreCalculator(null, new DateTimeAnswer(DateTime.Now)));
    }
}