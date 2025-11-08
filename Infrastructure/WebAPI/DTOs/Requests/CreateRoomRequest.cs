using System.ComponentModel.DataAnnotations;

namespace DataGuessr.Infrastructure.WebAPI.DTOs.Requests
{
    /// <summary>
    /// Запрос на создание новой комнаты
    /// </summary>
    public class CreateRoomRequest
    {
        [Required(ErrorMessage = "Настройки приватности обязательны")]
        public required string Privacy { get; set; } = "Private";
        // "Private" - только по ссылке-приглашению
        // "Public" - видна в списке комнат

        [Range(2, 10, ErrorMessage = "Максимальное количество игроков должно быть от 2 до 10")]
        public int MaxPlayers { get; set; } = 6;
        // Данные создателя берутся из контекста авторизации
    }
}