using Domain.Interfaces;

namespace Domain.Entities;


public class User : IEntity<Guid>
{
    public Guid Id { get; }
    public string Name { get; set; }
    public Guid AvatarId { get; set; }
    public string? PasswordHash { get; set; }
    public string? Login { get; set; } 

    public User(string name, Guid avatar, string? passwordHash = null, string? login = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        PasswordHash = passwordHash;
        AvatarId = avatar;
        Login = login;
    }
}
