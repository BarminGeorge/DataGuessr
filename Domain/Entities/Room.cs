using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Room : IEntity<Guid>
{
    public Guid Id { get; private set; }

    public virtual ICollection<Player> Players { get; private set; } = new List<Player>();
    public virtual ICollection<Game> Games { get; private set; } = new List<Game>();

    public Guid Owner { get; private set; }
    public RoomPrivacy Privacy { get; private set; }
    public string? Password { get; private set; }
    public RoomStatus Status { get; private set; }
    public int MaxPlayers { get; private set; }

    public DateTime ClosedAt { get; private set; }

    protected Room() { }

    public Room(Guid ownerId, RoomPrivacy privacy, int maxPlayers, string? password = null, TimeSpan? ttl = null)
    {
        Id = Guid.NewGuid();
        Owner = ownerId;
        Privacy = privacy;
        Status = RoomStatus.Available;
        Password = password;
        MaxPlayers = maxPlayers;

        ClosedAt = DateTime.UtcNow + (ttl ?? TimeSpan.FromDays(1));
    }

    public bool IsExpired => DateTime.UtcNow > ClosedAt;
    
    public void AddPlayer(Player player)
    {
        if (Status == RoomStatus.Archived)
            throw new InvalidOperationException("Комната архивирована");
        if (Players.Count >= MaxPlayers)
            throw new InvalidOperationException("Комната заполнена");

        Players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        if (Status == RoomStatus.Archived)
            throw new InvalidOperationException("Комната архивирована");
        
        Players.Remove(player);
    }

    public void AddGame(Game game)
    {
        Games.Add(game);
    }
    
    public void FillPlayersWithUserInfo(IEnumerable<User> users)
    {
        if (Players.Count == 0)
            return;

        var userDict = users.ToDictionary(u => u.Id, u => u);
            
        foreach (var player in Players)
            if (userDict.TryGetValue(player.UserId, out var user))
                player.SetUserInfo(user);
    }

    public void SetNewOwner()
    {
        if (Players.Count == 0)
            throw new InvalidOperationException("Коллекция игроков пуста.");

        var random = new Random();
        var randomIndex = random.Next(Players.Count);
        var player = new List<Player>(Players)[randomIndex];
        
        Owner = player.Id;
    }
}