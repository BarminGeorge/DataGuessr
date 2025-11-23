using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Player : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public User User { get; private set; }
    public Score Score { get; private set; }

    public Player(User user)
    {
        Id = user.Id;
        User = user;
        Score = Score.Zero;
    }

    public void UpdateScore(Score newScore)
    {
        Score = newScore;
    }
}