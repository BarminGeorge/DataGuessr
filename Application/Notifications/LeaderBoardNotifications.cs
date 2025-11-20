using Domain.ValueTypes;

namespace Application.Notifications;

public record LeaderBoardNotifications(Statistic Statistic) : GameNotification
{
    public override string MethodName => "ShowLeaderBoard";
}