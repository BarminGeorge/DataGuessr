using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Game : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public IMode Mode { get; private set; }
    public GameStatus Status { get; private set; }
    public Statistic CurrentStatistic { get; private set; }
    public IReadOnlyList<Question> Questions => questions.AsReadOnly();

    private readonly List<Question> questions = [];

    public Game(IMode mode)
    {
        Id = Guid.NewGuid();
        Mode = mode;
        CurrentStatistic = new Statistic();
        Status = GameStatus.NotStarted;
    }

    public void AddQuestion(Question question) => questions.Add(question);
    public void AddQuestions(IEnumerable<Question> question) => questions.AddRange(question);

    public void StartGame()
    {
        if (Status is GameStatus.NotStarted)
            Status = GameStatus.InProgress;
        throw new InvalidOperationException("Игра уже началась");
    }

    public void FinishGame()
    {
        if (Status is GameStatus.InProgress)
            Status = GameStatus.Finished;
        throw new InvalidOperationException("Игра еще не началась или уже закончена");
    }
}