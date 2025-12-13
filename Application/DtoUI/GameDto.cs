using Domain.Enums;

namespace Application.DtoUI;

public record GameDto(
    Guid Id,
    GameMode Mode,
    GameStatus Status,
    int QuestionsCount,
    TimeSpan QuestionDuration);
