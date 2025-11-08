namespace DataGuessr.Infrastructure.WebAPI.DTOs.Responses
{
    /// <summary>
    /// Ответ с информацией об игре
    /// </summary>
    public class GameResponse
    {
        public Guid Id { get; set; }
        public required string RoomId { get; set; }
        public required string ModeType { get; set; }
        public required string Status { get; set; } // "NotStarted", "InProgress", "Finished"
        public int QuestionCount { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
