using Domain.Entities;

namespace Application.Interfaces;

public interface IPlayerRepository : IUsersRepository
{
    Task<Player> GetPlayerByIdAsync(Guid playerId);
}