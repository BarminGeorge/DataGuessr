using Domain.Interfaces;

namespace Domain.Entities;


public class User : IEntity<Guid>
{
    public Guid Id { get; }
    public string Name { get; set; }
    public Avatar Avatar { get; set; }
    public string PasswordHash { get; set; }
    public User(string name, Avatar avatar, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
        PasswordHash = passwordHash;
        Avatar = avatar;
    }
}