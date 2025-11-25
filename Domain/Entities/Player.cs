using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Player : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid RoomId { get; private set; }
    public string? GuestName { get; private set; }
    public Guid? GuestAvatarId { get; private set; }
    public Score Score { get; private set; }
    public bool IsGuest => UserId == null;

    // Конструктор для авторизованного игрока
    public Player(Guid userId, Guid roomId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        GuestName = null;
        GuestAvatarId = null;
        Score = Score.Zero;
        RoomId = roomId;
    }

    // Конструктор для гостя
    public Player(string guestName, Guid guestAvatar, Guid roomId)
    {
        Id = Guid.NewGuid();
        UserId = null;
        GuestName = guestName;
        GuestAvatarId = guestAvatar;
        Score = Score.Zero;
        RoomId = roomId;
    }

    public void UpdateScore(Score newScore)
    {
        Score = newScore;
    }
}