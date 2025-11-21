using Domain.ValueTypes;

namespace Domain.Entities;

public class Player : Entity
{
    public Guid UserId { get; private set; } // Ссылка на пользователя
    public User User { get; private set; }   // Навигационное свойство
    public Score Score { get; private set; }

    public Player(User user)
    {
        UserId = user.Id;
        User = user;
        Score = Score.Zero;
    }

    public void UpdateScore(Score newScore)
    {
        Score = newScore;
    }
}
