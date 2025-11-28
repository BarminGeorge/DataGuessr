using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Room : IEntity<Guid>
{
    public Guid Id { get; private set; }

    // Навигационные свойства для EF Core
    public virtual ICollection<Player> Players { get; private set; } = new List<Player>();
    public virtual ICollection<Game> Games { get; private set; } = new List<Game>();

    public Guid Owner { get; private set; }
    public RoomPrivacy Privacy { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string? Password { get; private set; }
    public RoomStatus Status { get; private set; }
    public int MaxPlayers { get; private set; }

    // Конструктор для EF Core
    protected Room() { }

    public Room(Guid ownerId, RoomPrivacy privacy, int maxPlayers, string? password = null)
    {
        Id = Guid.NewGuid();
        Owner = ownerId;
        Privacy = privacy;
        Status = RoomStatus.Available;
        Password = password;
        Code = GenerateCode();
        MaxPlayers = maxPlayers;
    }

    public Game? CurrentGame() => Games.LastOrDefault();

    public bool HasOngoingGame()
    {
        var currentGame = CurrentGame();
        return currentGame?.Status == GameStatus.InProgress;
    }

    public bool IsFull() => Players.Count >= MaxPlayers;

    // Генерация кода комнаты
    private static string GenerateCode() => Guid.NewGuid().ToString("N")[..6];
}
