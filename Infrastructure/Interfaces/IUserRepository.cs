using Domain.Common;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<OperationResult> AddAsync(User user, CancellationToken ct = default);
    Task<OperationResult<User>> GetByNameAsync(string userName, CancellationToken ct = default);
}