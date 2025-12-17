using Domain.Enums;

namespace Application.DtoUI;

public record QuestionDto(
    Guid QuestionId,
    GameMode GameMode,
    string Formulation,
    string ImageUrl,
    DateTime EndTime,
    int DurationInSeconds);