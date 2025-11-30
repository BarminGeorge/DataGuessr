namespace Infrastructure.WebAPI.DTOs.Responses
{
    /// <summary>
    /// Ответ после создания комнаты
    /// </summary>
    public record CreateRoomResponse(
        Guid RoomId,
        string RoomCode,
        string ShareableUrl, // Готовая ссылка для копирования и отправки друзьям
        DateTime ExpiresAt, // Время жизни комнаты (например, 24 часа)
        string Message // Текст для UI: "Комната создана! Отправьте ссылку друзьям"
    )
    {
        public CreateRoomResponse() : this(Guid.Empty, "", "", DateTime.UtcNow, "") { }
    };
}