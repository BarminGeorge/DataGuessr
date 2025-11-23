using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.DtoUI;

public record GameDto(
    Guid Id,
    GameMode Mode,
    GameStatus Status,
    Statistic CurrentStatistic,
    IReadOnlyList<Question> Questions,
    int QuestionsCount,
    TimeSpan QuestionDuration);
