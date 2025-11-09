using Domain.Entities;

namespace Application.Interfaces;

public interface IPlayerRepository
{
    Task<Player> GetPlayerByIdAsync(Guid playerId);
}