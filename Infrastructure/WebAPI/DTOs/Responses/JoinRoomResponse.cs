namespace DataGuessr.Infrastructure.WebAPI.DTOs.Responses
{
    /// <summary>
    /// Ответ после успешного присоединения к комнате
    /// </summary>
    public class JoinRoomResponse
    {
        public Guid RoomId { get; set; }

        public string RoomCode { get; set; }
        // Для проверки что пользователь в правильной комнате

        public Guid PlayerId { get; set; }
        // Уникальный идентификатор игрока в этой сессии

        public string PlayerName { get; set; }
        // Подтверждение имени которое ввел пользователь

        public bool IsHost { get; set; }
        // Является ли этот игрок создателем комнаты

        public RoomStatusResponse RoomStatus { get; set; }
        // Текущее состояние комнаты и список игроков
    }
}