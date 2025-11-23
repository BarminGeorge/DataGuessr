using Domain.Interfaces;

namespace Domain.ValueTypes;

public record Question(Answer RightAnswer, string Formulation) : IEntity<Guid>
{
    public Guid Id { get; } = Guid.NewGuid();
    public IReadOnlyList<Answer> AllAnswers => allAnswers.AsReadOnly();

    private readonly List<Answer> allAnswers = [];

    public void AddAnswer(Answer answer) => allAnswers.Add(answer);
}