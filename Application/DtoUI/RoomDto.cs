using Domain.Entities;

namespace Application.DtoUI;

public record RoomDto(
    Guid Id,
    Guid Host,
    HashSet<Player> Players);