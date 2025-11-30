using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext db;

    public UserRepository(AppDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<OperationResult> AddAsync(User user, CancellationToken ct)
    {
        return await OperationResult.TryAsync(async () =>
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Проверяем уникальность логина
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
        });
    }

    public async Task<OperationResult<User>> GetByLoginAsync(string login, CancellationToken ct)
    {
        return await OperationResult<User>.TryAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login не может быть пустым", nameof(login));

            var user = await db.Users
                .AsNoTracking()
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Login == login, ct)
                ?? throw new KeyNotFoundException($"Пользователь с логином '{login}' не найден");

            return user;
        });
    }

    public async Task<OperationResult> UpdateUserAsync(
        Guid userId,
        Avatar avatar,
        string playerName,
        CancellationToken ct)
    {
        return await OperationResult.TryAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(playerName))
                throw new ArgumentException("PlayerName не может быть пустым", nameof(playerName));

            if (avatar == null)
                throw new ArgumentNullException(nameof(avatar));

            //Загружаем user с его текущей аватаркой
            var user = await db.Users
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == userId, ct)
                ?? throw new KeyNotFoundException($"Пользователь с ID '{userId}' не найден");

            var oldAvatar = user.Avatar;

            //Обновляем профиль
            user.UpdateProfile(playerName, avatar);

            //Удаляем старую аватарку (FK constraint автоматически обработает
            if (oldAvatar != null)
            {
                db.Avatars.Remove(oldAvatar);
            }

            db.Users.Update(user);
            await db.SaveChangesAsync(ct);
        });
    }

    public async Task<OperationResult<string>> GetPlayerNameByIdAsync(Guid playerId, CancellationToken ct)
    {
        return await OperationResult<string>.TryAsync(async () =>
        {
            var playerName = await db.Users
                .AsNoTracking()
                .Where(u => u.Id == playerId)
                .Select(u => u.PlayerName)
                .FirstOrDefaultAsync(ct)
                ?? throw new KeyNotFoundException($"Пользователь с ID '{playerId}' не найден");

            return playerName;
        });
    }
}
