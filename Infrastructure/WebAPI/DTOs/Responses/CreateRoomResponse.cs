namespace DataGuessr.Infrastructure.WebAPI.DTOs.Responses
{
    /// <summary>
    /// Ответ после создания комнаты
    /// </summary>
    public class CreateRoomResponse
    {
        public Guid RoomId { get; set; }

        public required string RoomCode { get; set; }

        public required string ShareableUrl { get; set; }
        // Готовая ссылка для копирования и отправки друзьям

        public DateTime ExpiresAt { get; set; }
        // Время жизни комнаты (например, 24 часа)


        public required string Message { get; set; }
        // Текст для UI: "Комната создана! Отправьте ссылку друзьям"
    }
}