using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IJwtProvider
{
    string GenerateTokenAsync(User user);
}