using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace Application.Services;
 
public class UserService(IJwtProvider provider, IUsersRepository usersRepository, IPasswordHasher passwordHasher)
{
    public async Task Register(string userName, string password)
    {
        var hashedPassword = passwordHasher.Generate(password);
        var user = new User(userName, hashedPassword);
        await usersRepository.Add(user);
    }

    public async Task<string> Login(string userName, string password)
    {
        var user = await usersRepository.GetByName(userName);
        var result = passwordHasher.Verify(password, user.PasswordHash);
        if (!result)
            throw new ApplicationException("Invalid username or password");
        
        var token = provider.GenerateToken(user);
        return token;
    }
}

// в инфраструктуру
public class PasswordHasher : IPasswordHasher
{
    public string Generate(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
}

// в инфраструктуру
public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions options = options.Value;

    public string GenerateToken(User user)
    {
        // TODO: пока небезопасно храниться в appsettings.json, переделать
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)), 
            SecurityAlgorithms.HmacSha256);

        Claim[] claims = [new("userId", user.Id.ToString())];
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(options.ExpairsHours));
        
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }
}

// в инфраструктуру
public class JwtOptions
{
    public string SecretKey { get; set; }
    public int ExpairsHours { get; set; }
}