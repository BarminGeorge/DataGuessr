using Domain.Common;

namespace Application.Notifications;

public interface INotificationService
{
    Task<OperationResult> NotifyGameRoomAsync<T>(Guid roomId, T notification) where T : GameNotification;
}