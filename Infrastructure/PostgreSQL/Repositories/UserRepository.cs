using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDataContext db;
    private readonly TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

    public UserRepository(IDataContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }


    public async Task<OperationResult<IEnumerable<User>>> GetUsersByIds(IEnumerable<Guid> usersId, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<IEnumerable<User>>>>(async () =>
        {
            if (usersId == null)
                return OperationResult<IEnumerable<User>>.Error.Validation("Список ID пользователей не может быть null");

            var userIdsList = usersId.ToList();

            if (userIdsList.Count == 0)
                return OperationResult<IEnumerable<User>>.Error.Validation("Список ID пользователей не может быть пустым");

            var users = await db.Users
                .AsNoTracking()
                .Include(u => u.Avatar)
                .Where(u => userIdsList.Contains(u.Id))
                .ToListAsync(ct);

            if (users.Count == 0)
                return OperationResult<IEnumerable<User>>.Error.NotFound("Пользователи с указанными ID не найдены");

            return OperationResult<IEnumerable<User>>.Ok(users);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }


    public async Task<OperationResult> AddAsync(User user, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (user == null)
                return OperationResult.Error.Validation("Пользователь не может быть null");

            if (!user.IsGuest)
            {
                var existingUser = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Login == user.Login, ct);

                if (existingUser != null)
                    return OperationResult.Error.AlreadyExists($"Пользователь с логином '{user.Login}' уже существует");
            }

            await db.Users.AddAsync(user, ct);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }


    public async Task<OperationResult<User>> GetByLoginAsync(string login, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<User>>>(async () =>
        {
            if (string.IsNullOrWhiteSpace(login))
                return OperationResult<User>.Error.Validation("Логин не может быть пустым");

            var user = await db.Users
                .AsNoTracking()
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Login == login, ct);

            if (user == null)
                return OperationResult<User>.Error.NotFound($"Пользователь с логином '{login}' не найден");

            return OperationResult<User>.Ok(user);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }

    public async Task<OperationResult> UpdateUserAsync(
    Guid userId,
    Avatar avatar,
    string playerName,
    CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult>>(async () =>
        {
            if (userId == Guid.Empty)
                return OperationResult.Error.Validation("UserId не может быть пустым GUID");

            if (string.IsNullOrWhiteSpace(playerName))
                return OperationResult.Error.Validation("PlayerName не может быть пустым");

            if (avatar == null)
                return OperationResult.Error.Validation("Avatar не может быть null");

            var user = await db.Users
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null)
                return OperationResult.Error.NotFound($"Пользователь с ID '{userId}' не найден");

            await db.Avatars.AddAsync(avatar, ct);

            user.UpdateProfile(playerName, avatar);

            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }




    public async Task<OperationResult<string>> GetPlayerNameByIdAsync(Guid playerId, CancellationToken ct)
    {
        var operation = new Func<Task<OperationResult<string>>>(async () =>
        {
            if (playerId == Guid.Empty)
                return OperationResult<string>.Error.Validation("PlayerId не может быть пустым GUID");

            var playerName = await db.Users
                .AsNoTracking()
                .Where(u => u.Id == playerId)
                .Select(u => u.PlayerName)
                .FirstOrDefaultAsync(ct);

            if (string.IsNullOrEmpty(playerName))
                return OperationResult<string>.Error.NotFound($"Пользователь с ID '{playerId}' не найден");

            return OperationResult<string>.Ok(playerName);
        });

        return await operation.WithRetry(maxRetries: 3, delay: retryDelay);
    }
}
