using Domain.ValueTypes;

namespace Domain.Entities;

public class Player : User
{
    public Score Score { get; set; }

    protected Player()
    {
    }

    public Player(User user)
        : base(user.Name, user.Login, user.PasswordHash, user.Avatar)
    {
        Score = Score.Zero;
    }
}