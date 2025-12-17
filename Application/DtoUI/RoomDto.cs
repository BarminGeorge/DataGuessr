namespace Application.DtoUI;

public record RoomDto(
    Guid Id,
    Guid OwnerId,
    IEnumerable<PlayerDto> Players,
    DateTime ClosedAt,
    string InviteCode);