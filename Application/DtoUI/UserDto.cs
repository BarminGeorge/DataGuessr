namespace Application.DtoUI;

public record UserDto(
    Guid Id,
    string PlayerName,
    string AvatarUrl);