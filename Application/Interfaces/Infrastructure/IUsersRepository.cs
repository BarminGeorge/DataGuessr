using Application.Result;
using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IUsersRepository
{
    Task<OperationResult> Add(User user);
    Task<OperationResult<User>> GetByName(string userName);
}