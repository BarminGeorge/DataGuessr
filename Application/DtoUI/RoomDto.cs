using Domain.Entities;

namespace Application.DtoUI;

public record RoomDto(
    Guid Id,
    Guid Host,
    List<Player> Players,
    DateTime ClosedAt);