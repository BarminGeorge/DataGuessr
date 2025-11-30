using Infrastructure.WebAPI.DTOs.Common;

namespace Infrastructure.WebAPI.DTOs.Responses
{
    /// <summary>
    /// Текущий статус комнаты
    /// </summary>
    public record RoomStatusResponse(
        string RoomCode,
        string Privacy,
        int MaxPlayers, // Для отображения "3/6 игроков"
        int CurrentPlayers, // Текущее количество игроков в комнате
        string Status,
        PlayerInfoDto Host, // Информация о создателе комнаты
        List<PlayerInfoDto> Players, // Список всех игроков currently в комнате
        bool CanStartGame // Можно ли начать игру (достаточно игроков, комната не заполнена)
    )
    {
        public RoomStatusResponse() : this(
            "", "", 0, 0, "Waiting",
            new PlayerInfoDto(), new List<PlayerInfoDto>(), false
        )
        { }
    };
}