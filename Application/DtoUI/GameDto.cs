using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.DtoUI;

public record GameDto(
    Guid Id,
    GameMode Mode,
    GameStatus Status,
    IReadOnlyList<QuestionDto> Questions,
    int QuestionsCount,
    TimeSpan QuestionDuration);
