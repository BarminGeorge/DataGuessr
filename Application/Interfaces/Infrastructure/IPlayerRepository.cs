using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IPlayerRepository : IUsersRepository
{
    Task<Player> GetPlayerByIdAsync(Guid playerId);
}