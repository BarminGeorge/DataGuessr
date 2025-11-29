using Domain.Interfaces;

namespace Domain.Entities;


public class User : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid AvatarId { get; private set; }
    public string PlayerName { get; private set; }
    public string? Login { get; private set; }
    public string? PasswordHash { get; private set; }

    public bool IsGuest => string.IsNullOrEmpty(Login);

    protected User() { }

    public User(string login, string playerName, Guid avatarId, string passwordHash)
    {
        Id = Guid.NewGuid();
        Login = login;
        PlayerName = playerName;
        AvatarId = avatarId;
        PasswordHash = passwordHash;
    }

    public User(string playerName, Guid avatarId)
    {
        Id = Guid.NewGuid();
        PlayerName = playerName;
        AvatarId = avatarId;
    }
}