using Domain.Enums;

namespace Infrastructure.WebAPI.DTOs.Responses
{
    /// <summary>
    /// Ответ с информацией об игре
    /// </summary>
    public record GameResponse(
        Guid Id,
        string RoomId,
        string ModeType,
        GameStatus Status,
        int QuestionCount,
        int CurrentQuestionIndex,
        DateTime CreatedAt
    )
    {
        // Пустой конструктор для десериализации
        public GameResponse() : this(
            Guid.Empty, "", "", GameStatus.NotStarted, 0, 0, DateTime.MinValue
        )
        {
        }
    }
}