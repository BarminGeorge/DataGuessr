using Application.Interfaces.Infrastructure;
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
    public async Task Register(string login, string password, string playerName, IFormFile image, CancellationToken ct)
    {
        var hashedPassword = passwordHasher.GenerateAsync(password);
        var operation = () => avatarRepository.SaveUserAvatarAsync(image, ct);
        var result = await operation.WithRetry(3, TimeSpan.FromSeconds(0.15));
        var avatar = result.ResultObj;
        var user = new User(login, playerName, avatar, hashedPassword);
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

    public async Task<OperationResult> UpdateUser(Guid userId, string PlayerName, IFormFile avatar,
        CancellationToken ct)
    {
        var operation = () => avatarRepository.SaveUserAvatarAsync(avatar, ct);
        var avatarResult = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!avatarResult.Success)
            return OperationResult.Error(avatarResult.ErrorMsg);

        return await OperationResult.TryAsync(() =>
            usersRepository.UpdateUserAsync(userId, avatarResult.ResultObj, PlayerName, ct));
    }

    public async Task<OperationResult<User>> CreateGuest(string playerName, IFormFile image, CancellationToken ct)
    {
        var operation = () => avatarRepository.SaveUserAvatarAsync(image, ct);
        var result = await operation.WithRetry(3, TimeSpan.FromSeconds(0.2));
        if (!result.Success)
            return OperationResult<User>.Error(result.ErrorMsg);

        var avatar = result.ResultObj;
        var user = new User(playerName, avatar);
        await usersRepository.AddAsync(user, ct);
        return OperationResult<User>.Ok(user);
    }
}