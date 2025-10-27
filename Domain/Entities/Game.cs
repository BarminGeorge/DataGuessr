using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Game : IEntity<Guid>
{
    public Guid Id { get; }
    public IMode Mode { get; }
    public Statistic CurrentStatistic { get; set; }
    public IReadOnlyList<Question> Questions => questions.AsReadOnly();

    private readonly List<Question> questions;

    public Game(IMode mode)
    {
        Mode = mode;
        CurrentStatistic = new Statistic();
        questions = [];
        Id = Guid.NewGuid();
    }

    public void AddQuestion(Question question) => questions.Add(question);
}