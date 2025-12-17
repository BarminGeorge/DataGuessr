using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services;

public class UserService(
    IJwtProvider provider, 
    IUserRepository userRepository, 
    IImageRepository imageRepository,
    IPasswordHasher passwordHasher)
    : IUserService
{
    public async Task<OperationResult<User>> Register(string login, string password, string playerName, IFormFile image, CancellationToken ct)
    {
        var hashedPassword = passwordHasher.Generate(password);
        var operation = () => imageRepository.SaveUserAvatarAsync(image, ct);
        var result = await operation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        if (!result.Success || result.ResultObj == null)
            return result.ConvertToOperationResult<User>();
        
        var user = new User(login, playerName, result.ResultObj, hashedPassword);
        var addOperation = () => userRepository.AddAsync(user, ct);
        var resultAddUser = await addOperation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        return resultAddUser.Success ? OperationResult<User>.Ok(user)
            : resultAddUser.ConvertToOperationResult<User>();
    }

    public async Task<OperationResult<(string token, User user)>> Login(string login, string password, CancellationToken ct)
    {
        var operation = () => userRepository.GetByLoginAsync(login, ct);
        var getUserResult = await operation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        if (getUserResult.ResultObj?.PasswordHash == null) 
            return getUserResult.ConvertToOperationResult<(string, User)>();
        
        var resultVerify = passwordHasher.Verify(password, getUserResult.ResultObj.PasswordHash);
        if (!resultVerify)
            return OperationResult<(string, User)>.Error.Unauthorized("Invalid username or password.");
        
        var token = provider.GenerateToken(getUserResult.ResultObj);
        var result = (token, getUserResult.ResultObj);
        return OperationResult<(string token, User userId)>.Ok(result);
    }
    
    public async Task<OperationResult<string>> UpdateUser(Guid userId, string playerName, IFormFile avatar,  CancellationToken ct)
    {
        var operation = () => imageRepository.SaveUserAvatarAsync(avatar, ct);
        var avatarResult = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!avatarResult.Success || avatarResult.ResultObj == null)
            return avatarResult.ConvertToOperationResult<string>();
     
        var updateOperation = () => userRepository.UpdateUserAsync(userId, avatarResult.ResultObj, playerName, ct);
        var updateOperationResult = await updateOperation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        if (!updateOperationResult.Success)
            return updateOperationResult.ConvertToOperationResult<string>();
        return new OperationResult<string>(true, avatarResult.ResultObj.Filename);
    }

    public async Task<OperationResult<User>> CreateGuest(string playerName, IFormFile image, CancellationToken ct)
    {
        var operation = () => imageRepository.SaveUserAvatarAsync(image, ct);
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