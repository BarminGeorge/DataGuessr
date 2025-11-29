using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Infrastructure;
using Application.Result;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace Application.Services;
 
public class UserService(
    IJwtProvider provider, 
    IUsersRepository usersRepository, 
    IPasswordHasher passwordHasher)
{
    public async Task Register(string login, string password, string playerName, IFormFile image, CancellationToken ct)
    {
        var hashedPassword = passwordHasher.GenerateAsync(password);
        var operation = () => usersRepository.SaveUserAvatar(image, ct);
        var result = await operation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        var avatar = result.ResultObj;
        var user = new User(login, playerName, avatar.Id, hashedPassword);
        await usersRepository.AddAsync(user, ct);
    }

    public async Task<string> Login(string login, string password, CancellationToken ct)
    {
        var user = await usersRepository.GetByLoginAsync(login, ct);
        var result = passwordHasher.VerifyAsync(password, user.ResultObj.PasswordHash);
        if (!result)
            throw new ApplicationException("Invalid username or password");
        
        var token = provider.GenerateTokenAsync(user.ResultObj);
        return token;
    }
    
    public async Task<OperationResult> UpdateUser(Guid userId, string userName, IFormFile avatar,  CancellationToken ct)
    {
        var operation = () => usersRepository.SaveUserAvatar(avatar, ct);
        var avatarResult = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!avatarResult.Success)
            return OperationResult.Error(avatarResult.ErrorMsg);

        return await OperationResult.TryAsync(() =>
            usersRepository.UpdateUser(userId, avatarResult.ResultObj.Id, userName, ct));
    }

    public async Task<OperationResult<User>> CreateGuest(string playerName, IFormFile image, CancellationToken ct)
    {
        var operation = () => usersRepository.SaveUserAvatar(image, ct);
        var result = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!result.Success)
            return OperationResult<User>.Error(result.ErrorMsg);
        
        var avatar = result.ResultObj;
        var user = new User(playerName, avatar.Id);
        await usersRepository.AddAsync(user, ct);
        return OperationResult<User>.Ok(user);
    }
}

// в инфраструктуру
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

// в инфраструктуру
public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions options = options.Value;

    public string GenerateTokenAsync(User user)
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