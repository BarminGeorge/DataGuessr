using Application.Result;
using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IUsersRepository
{
    Task<OperationResult> AddAsync(User user);
    Task<OperationResult<User>> GetByNameAsync(string userName);
}