using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Game : IEntity<Guid>
{
    public Guid Id { get; }
    public IMode Mode { get; }
    public GameStatus Status { get; private set; }
    public Statistic CurrentStatistic { get; set; }
    public IReadOnlyList<Question> Questions => questions.AsReadOnly();

    private readonly List<Question> questions;

    public Game(IMode mode)
    {
        Mode = mode;
        CurrentStatistic = new Statistic();
        questions = [];
        Id = Guid.NewGuid();
        Status = GameStatus.NotStarted;
    }

    public void AddQuestion(Question question) => questions.Add(question);

    public void StartGame()
    {
        if (Status is GameStatus.NotStarted)
            Status = GameStatus.InProgress;
        throw new InvalidOperationException("Игра уже началась");
    }

    public void FinishGame()
    {
        if  (Status is GameStatus.InProgress)
            Status = GameStatus.Finished;
        throw new InvalidOperationException("Игра еще не началась или уже закончена");
    }
}