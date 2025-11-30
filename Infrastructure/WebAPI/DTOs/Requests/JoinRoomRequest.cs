using System.ComponentModel.DataAnnotations;

namespace Infrastructure.WebAPI.DTOs.Requests
{
    /// <summary>
    /// Запрос на присоединение к комнате по ссылке
    /// </summary>
    public record JoinRoomRequest(
        [Required]
        [StringLength(6, MinimumLength = 6)]
        string RoomCode,

        [Required]
        [StringLength(20, MinimumLength = 2)]
        string PlayerName,

        string? AvatarUrl = null
    )
    {
        public JoinRoomRequest() : this("", "", null) { }
    };
}