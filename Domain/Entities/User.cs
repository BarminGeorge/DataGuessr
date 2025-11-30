using Domain.Interfaces;

namespace Domain.Entities;

public class User : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public string PlayerName { get; private set; }
    public string? Login { get; private set; }
    public string? PasswordHash { get; private set; }

    public virtual Avatar Avatar { get; set; }

    public bool IsGuest => string.IsNullOrEmpty(Login);

    protected User() { }

    public User(string login, string playerName, Avatar avatar, string passwordHash)
    {
        Id = Guid.NewGuid();
        Login = login;
        PlayerName = playerName;
        Avatar = avatar;
        PasswordHash = passwordHash;
    }

    public User(string playerName, Avatar avatar)
    {
        Id = Guid.NewGuid();
        PlayerName = playerName;
        Avatar = avatar;
    }

    public void UpdateProfile(string playerName, Avatar avatar)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            throw new ArgumentException("PlayerName не может быть пустым", nameof(playerName));

        PlayerName = playerName;
        Avatar = avatar;
    }
}
