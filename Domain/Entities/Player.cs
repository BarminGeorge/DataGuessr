using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Player : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid RoomId { get; private set; }
    public Score Score { get; private set; }

    public Player(Guid userId, Guid roomId)
    {
        Id = userId;
        UserId = userId;
        Score = Score.Zero;
        RoomId = roomId;
    }

    public void UpdateScore(Score newScore)
    {
        Score = newScore;
    }
}
