using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Game : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }

    public GameMode Mode { get; private set; }
    public GameStatus Status { get; private set; }
    public int QuestionsCount { get; private set; }
    public TimeSpan QuestionDuration { get; private set; }

    public Statistic? CurrentStatistic { get; set; }

    public virtual ICollection<Question> Questions { get; private set; } = new List<Question>();

    protected Game() { }

    public Game(Guid roomId, GameMode mode, TimeSpan questionDuration, int questionsCount)
    {
        Id = Guid.NewGuid();
        RoomId = roomId;
        Mode = mode;
        Status = GameStatus.NotStarted;
        QuestionDuration = questionDuration;
        QuestionsCount = questionsCount;
        CurrentStatistic = null;
    }

    public void StartGame()
    {
        if (Status != GameStatus.NotStarted)
            throw new InvalidOperationException("Игра уже началась");

        Status = GameStatus.InProgress;
    }

    public void FinishGame()
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Игра еще не началась или уже закончена");

        Status = GameStatus.Finished;
    }

    public void AddQuestions(IEnumerable<Question> questions)
    {
        foreach (var question in questions)
            Questions.Add(question);
    }
}
