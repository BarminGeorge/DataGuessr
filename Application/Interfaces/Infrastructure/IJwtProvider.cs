using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IJwtProvider
{
    string GenerateToken(User user);
}