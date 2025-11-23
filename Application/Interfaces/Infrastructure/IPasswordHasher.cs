namespace Application.Interfaces.Infrastructure;

public interface IPasswordHasher
{
    string GenerateAsync(string password);
    bool VerifyAsync(string password, string hashedPassword);
}