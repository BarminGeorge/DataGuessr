using Domain.Interfaces;

namespace Domain.Entities;


public class User : IEntity<Guid>
{
    public Guid Id { get; }
    public string Name { get; set; }
    public string Avatar { get; set; }

    public User(Guid id, string name, string avatar)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
    }
}