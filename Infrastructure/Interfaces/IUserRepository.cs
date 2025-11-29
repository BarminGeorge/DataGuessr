using Domain.Common;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<OperationResult> AddAsync(User user, CancellationToken ct);
    Task<OperationResult<User>> GetByLoginAsync(string login, CancellationToken ct);
    public Task<OperationResult> UpdateUserAsync(Guid userId, Guid avatarId, string playerName, CancellationToken ct);
    Task<OperationResult<string>> GetPlayerNameByIdAsync(Guid playerId, CancellationToken ct);
}