using Domain.Entities;

namespace Application.Interfaces;

public interface IQuickMatchService
{
    Task<Room> FindOrCreateQuickMatchAsync(Guid userId);
}