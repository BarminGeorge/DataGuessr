using Application.Result;
using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IUsersRepository
{
    Task<OperationResult> AddAsync(User user, CancellationToken ct);
    Task<OperationResult<User>> GetByLoginAsync(string login, CancellationToken ct);
    public Task<OperationResult> UpdateUser(Guid userId, IFormFile avatar, string userName, CancellationToken ct);
    public Task<OperationResult<Avatar>> SaveUserAvatar(IFormFile avatar, CancellationToken ct);
}