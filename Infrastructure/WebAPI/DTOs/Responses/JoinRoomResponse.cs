using Infrastructure.WebAPI.DTOs.Responses;

namespace DataGuessr.Infrastructure.WebAPI.DTOs.Responses
{
    /// <summary>
    /// Ответ после успешного присоединения к комнате
    /// </summary>
    public record JoinRoomResponse(
        Guid RoomId,
        string RoomCode, // Для проверки что пользователь в правильной комнате
        Guid PlayerId, // Уникальный идентификатор игрока в этой сессии
        string PlayerName, // Подтверждение имени которое ввел пользователь
        bool IsHost, // Является ли этот игрок создателем комнаты
        RoomStatusResponse RoomStatus // Текущее состояние комнаты и список игроков
    )
    {
        public JoinRoomResponse() : this(
            Guid.Empty, "", Guid.Empty, "", false, new RoomStatusResponse()
        )
        { }
    };
}