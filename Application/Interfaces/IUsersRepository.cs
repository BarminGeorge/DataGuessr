using Domain.Entities;

namespace Application.Interfaces;

public interface IUsersRepository
{
    Task<IResult> Add(User user);
    Task<User> GetByName(string userName);
}