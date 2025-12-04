using Infrastructure.Interfaces;

namespace Infrastructure.Providers;

public class PasswordHasher : IPasswordHasher
{
    public string GenerateAsync(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool VerifyAsync(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
}