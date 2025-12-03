using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services;
 
public class UserService(
    IJwtProvider provider, 
    IUserRepository usersRepository, 
    IAvatarRepository avatarRepository,
    IPasswordHasher passwordHasher)
{
    public async Task<OperationResult> Register(string login, string password, string playerName, IFormFile image, CancellationToken ct)
    {
        var hashedPassword = passwordHasher.GenerateAsync(password);
        var operation = () => avatarRepository.SaveUserAvatarAsync(image, ct);
        var result = await operation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        if (!result.Success || result.ResultObj == null)
            return OperationResult.Error($"{login} failed to register.");
        
        var user = new User(login, playerName, result.ResultObj, hashedPassword);
        return await usersRepository.AddAsync(user, ct);
    }

    public async Task<OperationResult<string>> Login(string login, string password, CancellationToken ct)
    {
        var getUserResult = await usersRepository.GetByLoginAsync(login, ct);
        if (getUserResult.ResultObj?.PasswordHash == null) 
            return OperationResult<string>.Error(getUserResult.ErrorMsg);
        
        var result = passwordHasher.VerifyAsync(password, getUserResult.ResultObj.PasswordHash);
        if (!result)
            return OperationResult<string>.Error("Invalid username or password.");
        
        var token = provider.GenerateTokenAsync(getUserResult.ResultObj);
        
        return OperationResult<string>.Ok(token);
    }
    
    public async Task<OperationResult> UpdateUser(Guid userId, string playerName, IFormFile avatar,  CancellationToken ct)
    {
        var operation = () => avatarRepository.SaveUserAvatarAsync(avatar, ct);
        var avatarResult = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!avatarResult.Success || avatarResult.ResultObj == null)
            return OperationResult.Error(avatarResult.ErrorMsg);

        return await OperationResult.TryAsync(() =>
            usersRepository.UpdateUserAsync(userId, avatarResult.ResultObj, playerName, ct));
    }

    public async Task<OperationResult<User>> CreateGuest(string playerName, IFormFile image, CancellationToken ct)
    {
        var operation = () => avatarRepository.SaveUserAvatarAsync(image, ct);
        var result = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!result.Success || result.ResultObj == null)
            return OperationResult<User>.Error(result.ErrorMsg);
        
        var avatar = result.ResultObj;
        var user = new User(playerName, avatar);
        await usersRepository.AddAsync(user, ct);
        return OperationResult<User>.Ok(user);
    }
}