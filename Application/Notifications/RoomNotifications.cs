using Domain.Entities;

namespace Application.Notifications;

public record NewPlayerNotification(Guid PlayerId, string PlayerName)
    : GameNotification
{
    public override string MethodName => "NewPlayerEntered";
}

public record PlayerLeavedNotification(Guid PlayerId, Guid HostId)
    : GameNotification
{
    public override string MethodName => "PlayerLeaved";
}

public record NewGameNotification(Game Game)
    : GameNotification
{
    public override string MethodName => "NewGameAdded";
}