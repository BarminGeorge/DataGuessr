using Domain.Interfaces;

namespace Domain.Entities;

public class User : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; private set; }
    public Avatar Avatar { get; set; }

    protected User()
    {
    }

    public User(string name, string login, string passwordHash, Avatar avatar)
    {
        Id = Guid.NewGuid();
        Name = name;
        Login = login;
        PasswordHash = passwordHash;
        Avatar = avatar;
    }
}