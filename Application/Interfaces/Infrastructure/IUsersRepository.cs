using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IUsersRepository
{
    Task<IResult> Add(User user);
    Task<User> GetByName(string userName);
}