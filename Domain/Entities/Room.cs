using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Room : IEntity<Guid>
{
    public Guid Id { get; }
    public RoomPrivacy Privacy { get; private set; }
    public int MaxPlayers { get; private set; }
    public DateTime CreatedAt { get; }
    public Player Host { get; } // Создатель комнаты
    public IReadOnlyList<Game> Games => games.AsReadOnly();
    public HashSet<Player> Players => new();

    private readonly List<Game> games = [];

    public Room(RoomPrivacy privacy, int maxPlayers, Player host)
    {
        Id = Guid.NewGuid();
        Privacy = privacy;
        MaxPlayers = maxPlayers;
        Host = host;
        CreatedAt = DateTime.UtcNow;

        // Создатель автоматически добавляется в комнату
        Players.Add(host);
    }

    public void AddGame(Game game)
    {
        if (games.Count > 0 && games[^1].Status is not GameStatus.Finished)
            throw new InvalidOperationException($"Нельзя начать новую игру. Последняя игра еще не окончена {games[^1].Status}");

        games.Add(game);
    }

    public void AddPlayer(Player player)
    {
        if (Players.Count >= MaxPlayers)
            throw new InvalidOperationException("Комната заполнена");

        Players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);
    }
}