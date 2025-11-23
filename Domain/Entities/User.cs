using Domain.Interfaces;

namespace Domain.Entities;


public class User : IEntity<Guid>
{
    public Guid Id { get; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public string PasswordHash { get; set; }

    public User(string name, string avatar)
    {
        Id = new Guid();
        Name = name;
        Avatar = avatar;
    }
}