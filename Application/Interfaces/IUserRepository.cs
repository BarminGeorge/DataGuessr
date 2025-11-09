using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<Player> GetPlayerByIdAsync(Guid playerId);
}