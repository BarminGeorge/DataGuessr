using Domain.Interfaces;

namespace Domain.Entities;

public class Avatar : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Filename { get; private set; }
    public string Mimetype { get; private set; }

    public virtual User User { get; set; }

    protected Avatar() { }

    public Avatar(string filename, string mimetype)
    {
        Id = Guid.NewGuid();
        Filename = filename;
        Mimetype = mimetype;
    }
}
