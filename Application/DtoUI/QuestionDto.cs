using Domain.Enums;

namespace Application.DtoUI;

public record QuestionDto(
    GameMode GameMode,
    string Formulation,
    string ImageUrl,
    DateTime EndTime,
    int DurationInSeconds);