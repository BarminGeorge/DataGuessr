using Domain.Interfaces;

namespace Domain.Entities;


public class User : IEntity<Guid>
{
    public Guid Id { get; }
    public Guid AvatarId { get; set; }
    public string PlayerName { get; }
    public string Login { get; }
    
    public string PasswordHash { get; set; }
    
    public User(string login, string playerName, Guid avatarId, string passwordHash)
    {
        Id = Guid.NewGuid();
        Login = login;
        PlayerName = playerName;
        PasswordHash = passwordHash;
        AvatarId = avatarId;
    }
}