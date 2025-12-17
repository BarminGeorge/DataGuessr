namespace Application.DtoUI;

public record PlayerDto(
    Guid PlayerId,
    Guid UserId,
    string PlayerName,
    string AvatarUrl);