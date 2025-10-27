using Domain.ValueTypes;

namespace Domain.Entities;

public class Player : User
{
    public Score Score { get; set; }

    public Player(Guid id, string name, byte[] avatar) 
        : base(id, name, avatar)
    {
        Score = Score.Zero;
    }
}