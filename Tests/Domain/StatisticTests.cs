/*using Domain.ValueTypes;

namespace Tests.Domain;

[TestFixture]
public class ScoreTests
{
    [TestCase(0, 0, 0)]
    [TestCase(1, 2, 3)]
    [TestCase(-1, 1, 0)]
    [TestCase(2.5, 3.5, 6)]
    public void Score_Addition_ReturnsCorrectResult(double a, double b, double expected)
    {
        var scoreA = new Score(a);
        var scoreB = new Score(b);

        var result = scoreA + scoreB;

        Assert.That(result.score, Is.EqualTo(expected));
    }

    [TestCase(0, 0, 0)]
    [TestCase(5, 3, 2)]
    [TestCase(3, 5, -2)]
    [TestCase(4.5, 1.5, 3)]
    public void Score_Subtraction_ReturnsCorrectResult(double a, double b, double expected)
    {
        var scoreA = new Score(a);
        var scoreB = new Score(b);

        var result = scoreA - scoreB;
            
        Assert.That(result.score, Is.EqualTo(expected));
    }

    [Test]
    public void Score_Zero_ReturnsZeroScore()
    {
        var zero = Score.Zero;
        Assert.That(zero.score, Is.EqualTo(0));
    }
}

[TestFixture]
public class StatisticTests
{
    private Guid userId1;
    private Guid userId2;
    private Guid userId3;

    [SetUp]
    public void SetUp()
    {
        userId1 = Guid.NewGuid();
        userId2 = Guid.NewGuid();
        userId3 = Guid.NewGuid();
    }

    [Test]
    public void Statistic_Update_AddsNewScoresForNewUsers()
    {
        var statistic = new Statistic();
        var answers = new Dictionary<Guid, Answer>
        {
            { userId1, new DateTimeAnswer(DateTime.Now) }
        };
        var rightAnswer = new DateTimeAnswer(DateTime.Now.AddMinutes(1));
            
        statistic.Update(answers, rightAnswer, (answer, right) => 
            new Score(answer < right ? 1 : 0));

        Assert.That(statistic.Scores, Has.Count.EqualTo(1));
        Assert.That(statistic.Scores[userId1].score, Is.EqualTo(1));
    }

    [Test]
    public void Statistic_Update_UpdatesExistingScores()
    {
        var statistic = new Statistic();
        var initialAnswers = new Dictionary<Guid, Answer>
        {
            { userId1, new DateTimeAnswer(DateTime.Now) }
        };
        var rightAnswer1 = DateTime.Now.AddMinutes(1);
        statistic.Update(initialAnswers, rightAnswer1, (answer, right) => 
            new Score(answer.Value < right ? 1 : 0));

        var updatedAnswers = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now.AddMinutes(2)) }
        };
        var rightAnswer2 = DateTime.Now.AddMinutes(3);

        statistic.Update(updatedAnswers, rightAnswer2, (answer, right) => 
            new Score(answer.Value < right ? 2 : 0));

        Assert.That(statistic.Scores.Count, Is.EqualTo(1));
        Assert.That(statistic.Scores[userId1].score, Is.EqualTo(3)); // 1 + 2
    }

    [Test]
    public void Statistic_Update_MultipleUsers_UpdatesCorrectly()
    {
        var statistic = new Statistic();
        var answers = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now) },
            { userId2, new Answer(DateTime.Now) }
        };
        var rightAnswer = DateTime.Now.AddMinutes(1);

        statistic.Update(answers, rightAnswer, (answer, right) => 
            new Score(answer.Value < right ? 1 : 0));

        Assert.That(statistic.Scores.Count, Is.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(statistic.Scores[userId1].score, Is.EqualTo(1));
            Assert.That(statistic.Scores[userId2].score, Is.EqualTo(1));
        });
    }

    [Test]
    public void Statistic_Diff_ReturnsCorrectDifferences()
    {
        var stat1 = new Statistic();
        var stat2 = new Statistic();

        var answers1 = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now) },
            { userId2, new Answer(DateTime.Now) }
        };
        var rightAnswer1 = DateTime.Now.AddMinutes(1);

        var answers2 = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now) },
            { userId3, new Answer(DateTime.Now) }
        };
        var rightAnswer2 = DateTime.Now.AddMinutes(1);

        stat1.Update(answers1, rightAnswer1, (answer, right) => 
            new Score(answer.Value < right ? 1 : 0));
        stat2.Update(answers2, rightAnswer2, (answer, right) => 
            new Score(answer.Value < right ? 2 : 0));

        var diff = stat1.Diff(stat2);

        Assert.That(diff.Scores.Count, Is.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(diff.Scores[userId1].score, Is.EqualTo(-1)); // 1 - 2
            Assert.That(diff.Scores[userId2].score, Is.EqualTo(1));  // 1 - 0
            Assert.That(diff.Scores[userId3].score, Is.EqualTo(-2)); // 0 - 2
        });
    }

    [Test]
    public void Statistic_OperatorSubtraction_ReturnsCorrectDifferences()
    {
        var stat1 = new Statistic();
        var stat2 = new Statistic();

        var answers1 = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now) }
        };
        var rightAnswer1 = DateTime.Now.AddMinutes(1);

        var answers2 = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now) }
        };
        var rightAnswer2 = DateTime.Now.AddMinutes(1);

        stat1.Update(answers1, rightAnswer1, (_, _) => 
            new Score(5));
        stat2.Update(answers2, rightAnswer2, (_, _) => 
            new Score(3));
            
        var result = stat1 - stat2;

        Assert.That(result.Scores, Has.Count.EqualTo(1));
        Assert.That(result.Scores[userId1].score, Is.EqualTo(2));
    }

    [Test]
    public void Statistic_Copy_ReturnsNewInstanceWithSameValues()
    {
        var original = new Statistic();
        var answers = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now) },
            { userId2, new Answer(DateTime.Now) }
        };
        var rightAnswer = DateTime.Now.AddMinutes(1);
            
        original.Update(answers, rightAnswer, (answer, right) => 
            new Score(answer.Value < right ? 1 : 0));

        var copy = original.Copy();

        Assert.That(copy, Is.Not.SameAs(original));
        Assert.That(copy.Scores, Has.Count.EqualTo(original.Scores.Count));
        Assert.Multiple(() =>
        {
            Assert.That(copy.Scores[userId1].score, Is.EqualTo(original.Scores[userId1].score));
            Assert.That(copy.Scores[userId2].score, Is.EqualTo(original.Scores[userId2].score));
        });
    }

    [Test]
    public void Statistic_Copy_IsIndependentFromOriginal()
    {
        var original = new Statistic();
        var answers = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now) }
        };
        var rightAnswer = DateTime.Now.AddMinutes(1);
            
        original.Update(answers, rightAnswer, (_, _) => 
            new Score(1));
            
        var copy = original.Copy();

        var newAnswers = new Dictionary<Guid, Answer>
        {
            { userId2, new Answer(DateTime.Now) }
        };
        original.Update(newAnswers, rightAnswer, (_, _) => 
            new Score(2));

        Assert.Multiple(() =>
        {
            Assert.That(original.Scores, Has.Count.EqualTo(2));
            Assert.That(copy.Scores, Has.Count.EqualTo(1));
        });
        Assert.That(copy.Scores.ContainsKey(userId2), Is.False);
    }

    [Test]
    public void Statistic_Diff_WhenOneStatisticEmpty_ReturnsOtherStatisticsValues()
    {
        var emptyStat = new Statistic();
        var statWithValues = new Statistic();
            
        var answers = new Dictionary<Guid, Answer>
        {
            { userId1, new Answer(DateTime.Now) }
        };
        var rightAnswer = DateTime.Now.AddMinutes(1);
            
        statWithValues.Update(answers, rightAnswer, (_, _) => 
            new Score(5));

        var diff = statWithValues.Diff(emptyStat);

        Assert.That(diff.Scores, Has.Count.EqualTo(1));
        Assert.That(diff.Scores[userId1].score, Is.EqualTo(5));
    }
}*/