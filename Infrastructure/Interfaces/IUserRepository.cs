using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task AddAsync(User user, CancellationToken ct = default);
    Task<User?> GetByNameAsync(string userName, CancellationToken ct = default);
}