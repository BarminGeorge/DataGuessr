using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Room : IEntity<Guid>
{
    public Guid Id { get; }
    public IReadOnlyList<Game> Games => games.AsReadOnly();
    public HashSet<Player> Players => new();
    public Guid Owner { get; private set; }
    public RoomPrivacy Privacy { get; private set; }
    public DateTime ClosedAt { get; }
    
    public string? Password { get; private set; } = null;
    public RoomStatus Status { get; private set; }
    private int maxPlayers;

    public int MaxPlayers
    {
        get => maxPlayers;
        set
        {
            if (Status == RoomStatus.Archived)
                throw new InvalidOperationException("Комната архивирована");
            if (HasOngoingGame())
                throw new InvalidOperationException("В комнате сейчас идет игра, нельзя изменять настройки комнаты");
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            maxPlayers = value;
        }
    }
    private readonly List<Game> games = [];
    private readonly HashSet<Player> players = [];

    protected Room()
    {
    }

    public Room(Guid ownerId, RoomPrivacy privacy, int maxPlayers, string? password = null)
    {
        Id = Guid.NewGuid();
        Owner = ownerId;
        Privacy = privacy;
        Status = RoomStatus.Available;
        Password = password;
        MaxPlayers = maxPlayers;
        ClosedAt = DateTime.Now + TimeSpan.FromDays(1);
    }

    public void AddGame(Game game)
    {
        if (Status == RoomStatus.Archived)
            throw new InvalidOperationException("Комната архивирована");
        if (HasOngoingGame())
            throw new InvalidOperationException("Нельзя начать новую игру. Последняя игра еще не окончена");
        ArgumentNullException.ThrowIfNull(game);

        games.Add(game);
    }

    public void AddPlayer(Player player)
    {
        if (Status == RoomStatus.Archived)
            throw new InvalidOperationException("Комната архивирована");
        if (players.Count >= MaxPlayers)
            throw new InvalidOperationException("Комната заполнена");

        players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        if (Status == RoomStatus.Archived)
            throw new InvalidOperationException("Комната архивирована");
        ArgumentNullException.ThrowIfNull(player);
        players.Remove(player);
    }

    public void ArchiveRoom()
    {
        Status = RoomStatus.Archived;
        players.Clear();
    }

    public bool IsFull()
    {
        return games.Count == MaxPlayers;
    }

    public Game? CurrentGame()
    {
        if (Status == RoomStatus.Archived)
            throw new InvalidOperationException("Комната архивирована");
        return Games.LastOrDefault();
    }

    public bool HasOngoingGame()
    {
        if (Status == RoomStatus.Archived)
            throw new InvalidOperationException("Комната архивирована");
        var currentGame = CurrentGame();
        if (currentGame is null) return false;
        return currentGame.Status == GameStatus.InProgress;
    }
}