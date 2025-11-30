using System.ComponentModel.DataAnnotations;

namespace Infrastructure.WebAPI.DTOs.Requests
{
    /// <summary>
    /// Запрос на создание новой игры в комнате
    /// </summary>
    public record CreateGameRequest(
        [Required] string RoomId,
        [Required] string ModeType,
        [Range(1, 20)] int QuestionCount = 5
    )
    {
        // Пустой конструктор для десериализации
        public CreateGameRequest() : this("", "DefaultMode", 5) { }
    }
}