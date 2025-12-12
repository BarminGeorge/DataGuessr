using Domain.Enums;

namespace Application.DtoUI;

public record GameDto(
    Guid Id,
    GameMode Mode,
    GameStatus Status,
    IReadOnlyList<QuestionDto> Questions,
    int QuestionsCount,
    TimeSpan QuestionDuration);
