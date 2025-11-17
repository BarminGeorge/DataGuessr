using Domain.Entities;

namespace Application.Interfaces;

public interface IQuickRoomService
{
    Task<Room> FindOrCreateQuickRoomAsync(Guid userId);
}