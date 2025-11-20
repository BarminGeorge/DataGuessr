namespace Application.Notifications;

public interface INotificationService
{
    Task NotifyGameRoomAsync<T>(Guid roomId, T notification) where T : GameNotification;
}