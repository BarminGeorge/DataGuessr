namespace Application.Notifications;

public abstract record GameNotification
{
    public abstract string MethodName { get; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}