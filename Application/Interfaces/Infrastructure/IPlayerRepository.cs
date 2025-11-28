using Application.Result;
using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IPlayerRepository : IUsersRepository
{
    Task<OperationResult<Player>> GetPlayerByIdAsync(Guid playerId, CancellationToken ct);
}