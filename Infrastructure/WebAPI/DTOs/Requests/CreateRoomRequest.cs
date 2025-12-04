using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Infrastructure.WebAPI.DTOs.Requests
{
    /// <summary>
    /// Запрос на создание новой комнаты
    /// </summary>
    public record CreateRoomRequest(
        [Required] RoomPrivacy Privacy = RoomPrivacy.Private,
        [Range(2, 10)] int MaxPlayers = 6
    )
    // Данные создателя берутся из контекста авторизации
    {
        public CreateRoomRequest() : this(RoomPrivacy.Private, 6) { }
    };
}