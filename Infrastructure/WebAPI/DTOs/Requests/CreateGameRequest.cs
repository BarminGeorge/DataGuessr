using System.ComponentModel.DataAnnotations;

namespace DataGuessr.Infrastructure.WebAPI.DTOs.Requests
{
    /// <summary>
    /// Запрос на создание новой игры в комнате
    /// </summary>
    public class CreateGameRequest
    {
        [Required(ErrorMessage = "RoomId обязателен")]
        public required string RoomId { get; set; }

        [Required(ErrorMessage = "Тип режима игры обязателен")]
        public required string ModeType { get; set; } = "DefaultMode";

        [Range(1, 20, ErrorMessage = "Количество вопросов должно быть от 1 до 20")]
        public int QuestionCount { get; set; } = 5;
    }
}