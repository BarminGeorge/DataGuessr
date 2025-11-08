namespace DataGuessr.Infrastructure.WebAPI.DTOs.Responses
{
    /// <summary>
    /// Текущий статус комнаты
    /// </summary>
    public class RoomStatusResponse
    {
        public string RoomCode { get; set; }

        public string Privacy { get; set; }

        public int MaxPlayers { get; set; }
        // Для отображения "3/6 игроков"

        public int CurrentPlayers { get; set; }
        // Текущее количество игроков в комнате

        public string Status { get; set; }
        // "Waiting" - ожидание игроков, "InGame" - игра идет

        public PlayerInfoDto Host { get; set; }
        // Информация о создателе комнаты

        public List<PlayerInfoDto> Players { get; set; } = new();
        // Список всех игроков currently в комнате

        public bool CanStartGame { get; set; }
        // Можно ли начать игру (достаточно игроков, комната не заполнена)
    }

    public class PlayerInfoDto
    {
        public Guid PlayerId { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public bool IsReady { get; set; }
        // Готов ли игрок к началу (можно добавить фичу "готовность")

        public DateTime JoinedAt { get; set; }
        // Когда игрок присоединился (для отображения "только что")
    }
}