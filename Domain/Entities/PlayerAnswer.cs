using Domain.Interfaces;
using Domain.ValueTypes;


namespace Domain.Entities;

public class PlayerAnswer : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public Guid PlayerId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Answer Answer { get; private set; }

    protected PlayerAnswer() { }

    public PlayerAnswer(Guid gameId, Guid playerId, Guid questionId, Answer answer)
    {
        Id = Guid.NewGuid();
        GameId = gameId;
        PlayerId = playerId;
        QuestionId = questionId;
        Answer = answer;
    }
}