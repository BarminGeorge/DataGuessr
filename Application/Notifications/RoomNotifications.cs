using Application.DtoUI;
using Domain.Entities;

namespace Application.Notifications;

public record NewPlayerNotification(PlayerDto Player)
    : GameNotification
{
    public override string MethodName => "NewPlayerEntered";
}

public record PlayerLeavedNotification(Guid PlayerId, Guid OwnerId)
    : GameNotification
{
    public override string MethodName => "PlayerLeaved";
}

public record NewGameNotification(GameDto Game)
    : GameNotification
{
    public override string MethodName => "NewGameAdded";
}

public record ReturnToRoomNotification(RoomDto Room)
    : GameNotification
{
    public override string MethodName => "ReturnToRoom";
}