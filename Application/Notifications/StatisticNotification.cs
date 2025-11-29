using Domain.ValueTypes;

namespace Application.Notifications;

public record StatisticNotification(Statistic Statistic) : GameNotification
{
    public override string MethodName => "ShowLeaderBoard";
}