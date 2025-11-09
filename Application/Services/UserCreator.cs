using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class UserCreator(IPlayerRepository userRepository)
{
    public async Task<User> Create(string name, string avatar)
    {
        var user = new User(name, avatar);
        await userRepository.AddAsync(user);
        return user;
    }
}