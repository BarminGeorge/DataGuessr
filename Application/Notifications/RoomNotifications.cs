using Domain.Entities;

namespace Application.Notifications;

public record NewPlayerNotification(Guid PlayerId, string PlayerName)
    : GameNotification
{
    public override string MethodName => "NewPlayerEntered";
}

public record PlayerLeavedNotification(Guid PlayerId, Guid OwnerId)
    : GameNotification
{
    public override string MethodName => "PlayerLeaved";
}

public record NewGameNotification(Game Game)
    : GameNotification
{
    public override string MethodName => "NewGameAdded";
}

public record ReturnToRoomNotification(Room Room)
    : GameNotification
{
    public override string MethodName => "ReturnToRoom";
}