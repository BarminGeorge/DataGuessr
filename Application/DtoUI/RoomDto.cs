namespace Application.DtoUI;

public record RoomDto(
    Guid Id,
    Guid Host,
    List<UserDto> Players,
    DateTime ClosedAt);