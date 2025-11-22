using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Room : IEntity<Guid>
{
    public Guid Id { get; }
    public IReadOnlyList<Game> Games => games.AsReadOnly();
    public HashSet<Player> Players => new();
    public Guid Owner { get; private set; }

    private readonly List<Game> games = [];

    public Room(Guid ownerId)
    {
        Id = Guid.NewGuid();
        Owner = ownerId;
    }
    
    public void AddGame(Game game)
    { 
        if (games[^1].Status is GameStatus.Finished)
            games.Add(game);
        throw new InvalidOperationException($"Нельзя начать новую игру. Последняя игра еще не окончена {games[^1].Status}");
    }
        
    public void AddPlayer(Player player) => Players.Add(player);

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);
        if (player.Id == Owner)
            Owner = Players.ElementAt(0).Id;
    }
}