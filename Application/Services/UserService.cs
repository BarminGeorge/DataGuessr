using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services;

public class UserService(
    IJwtProvider provider, 
    IUserRepository userRepository, 
    IAvatarRepository avatarRepository,
    IPasswordHasher passwordHasher)
    : IUserService
{
    public async Task<OperationResult> Register(string login, string password, string playerName, IFormFile image, CancellationToken ct)
    {
        var hashedPassword = passwordHasher.Generate(password);
        var operation = () => avatarRepository.SaveUserAvatarAsync(image, ct);
        var result = await operation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        if (!result.Success || result.ResultObj == null)
            return result;
        
        var user = new User(login, playerName, result.ResultObj, hashedPassword);
        var addOperation = () => userRepository.AddAsync(user, ct);
        return await addOperation.WithRetry(3, TimeSpan.FromSeconds(0.15));
    }

    public async Task<OperationResult<(string token, Guid userId)>> Login(string login, string password, CancellationToken ct)
    {
        var operation = () => userRepository.GetByLoginAsync(login, ct);
        var getUserResult = await operation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        if (getUserResult.ResultObj?.PasswordHash == null) 
            return getUserResult.ConvertToOperationResult<(string, Guid)>();
        
        var resultVerify = passwordHasher.Verify(password, getUserResult.ResultObj.PasswordHash);
        if (!resultVerify)
            return OperationResult<(string, Guid)>.Error.Unauthorized("Invalid username or password.");
        
        var token = provider.GenerateToken(getUserResult.ResultObj);
        var result = (token, getUserResult.ResultObj.Id);
        return OperationResult<(string token, Guid userId)>.Ok(result);
    }
    
    public async Task<OperationResult> UpdateUser(Guid userId, string playerName, IFormFile avatar,  CancellationToken ct)
    {
        var operation = () => avatarRepository.SaveUserAvatarAsync(avatar, ct);
        var avatarResult = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!avatarResult.Success || avatarResult.ResultObj == null)
            return avatarResult;

        var updateOperation = () => userRepository.UpdateUserAsync(userId, avatarResult.ResultObj, playerName, ct);
        return await updateOperation.WithRetry(3, TimeSpan.FromSeconds(0.15));
    }

    public async Task<OperationResult<User>> CreateGuest(string playerName, IFormFile image, CancellationToken ct)
    {
        var operation = () => avatarRepository.SaveUserAvatarAsync(image, ct);
        var saveAvatarResult = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!saveAvatarResult.Success || saveAvatarResult.ResultObj == null)
            return saveAvatarResult.ConvertToOperationResult<User>();

        var user = new User(playerName, saveAvatarResult.ResultObj);
        var addUserOperation = () => userRepository.AddAsync(user, ct);
        var result = await addUserOperation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        return result.Success 
            ? OperationResult<User>.Ok(user)
            : result.ConvertToOperationResult<User>();
    }
}