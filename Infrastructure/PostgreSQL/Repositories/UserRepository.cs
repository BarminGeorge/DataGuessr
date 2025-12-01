using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public UserRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult> AddAsync(User user, CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!string.IsNullOrEmpty(user.Login))
            {
                var existingUser = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Login == user.Login, ct);

                if (existingUser != null)
                    throw new InvalidOperationException($"Пользователь с логином '{user.Login}' уже существует");
            }

            await db.Users.AddAsync(user, ct);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult<User>> GetByLoginAsync(string login, CancellationToken ct)
    {
        Func<Task<OperationResult<User>>> operation = async () =>
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login не может быть пустым", nameof(login));

            var user = await db.Users
                .AsNoTracking()
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Login == login, ct)
                ?? throw new KeyNotFoundException($"Пользователь с логином '{login}' не найден");

            return OperationResult<User>.Ok(user);
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult> UpdateUserAsync(
        Guid userId,
        Avatar avatar,
        string playerName,
        CancellationToken ct)
    {
        Func<Task<OperationResult>> operation = async () =>
        {
            if (string.IsNullOrWhiteSpace(playerName))
                throw new ArgumentException("PlayerName не может быть пустым", nameof(playerName));

            if (avatar == null)
                throw new ArgumentNullException(nameof(avatar));

            var user = await db.Users
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == userId, ct)
                ?? throw new KeyNotFoundException($"Пользователь с ID '{userId}' не найден");

            var oldAvatar = user.Avatar;

            user.UpdateProfile(playerName, avatar);

            if (oldAvatar != null)
            {
                db.Avatars.Remove(oldAvatar);
            }

            db.Users.Update(user);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }

    public async Task<OperationResult<string>> GetPlayerNameByIdAsync(Guid playerId, CancellationToken ct)
    {
        Func<Task<OperationResult<string>>> operation = async () =>
        {
            var playerName = await db.Users
                .AsNoTracking()
                .Where(u => u.Id == playerId)
                .Select(u => u.PlayerName)
                .FirstOrDefaultAsync(ct)
                ?? throw new KeyNotFoundException($"Пользователь с ID '{playerId}' не найден");

            return OperationResult<string>.Ok(playerName);
        };

        return await operation.WithRetry(maxRetries: 3, retryDelay);
    }
}
