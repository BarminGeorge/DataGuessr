using Domain.Interfaces;

namespace Domain.Entities;

public class Room : IEntity<Guid>
{
    public Guid Id { get; }
    public IReadOnlyList<Game> Games => games.AsReadOnly();
    public HashSet<Player> Players => new();

    private readonly List<Game> games = [];

    public Room()
    {
        Id = Guid.NewGuid();
    }
    
    public void AddGame(Game game) => games.Add(game);
        
    public void AddPlayer(Player player) => Players.Add(player);
}