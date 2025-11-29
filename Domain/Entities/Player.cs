using Domain.Interfaces;

namespace Domain.Entities;

public class Player : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid RoomId { get; private set; }
    public string ConnectionId { get; private set; }

    protected Player() { }

    public Player(Guid userId, Guid roomId, string connectionId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        RoomId = roomId;
        ConnectionId = connectionId;
    }
}
