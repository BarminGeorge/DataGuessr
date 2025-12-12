namespace Application.DtoUI;

public record RoomDto(
    Guid Id,
    Guid Host,
    IEnumerable<PlayerDto> Players,
    DateTime ClosedAt);