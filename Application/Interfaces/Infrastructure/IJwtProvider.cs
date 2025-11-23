using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IJwtProvider
{
    string GenerateTokenAsync(User user);
}