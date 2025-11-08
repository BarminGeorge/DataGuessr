using System.ComponentModel.DataAnnotations;

namespace DataGuessr.Infrastructure.WebAPI.DTOs.Requests
{
    /// <summary>
    /// Запрос на присоединение к комнате по ссылке
    /// </summary>
    public class JoinRoomRequest
    {
        [Required(ErrorMessage = "Код комнаты обязателен")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Код комнаты должен быть 6 символов")]
        public string RoomCode { get; set; }

        [Required(ErrorMessage = "Имя игрока обязательно")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Имя должно быть от 2 до 20 символов")]
        public string PlayerName { get; set; }

        public string AvatarUrl { get; set; }
    }
}