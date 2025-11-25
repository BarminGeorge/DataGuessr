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

    public async Task<OperationResult> AddAsync(User user, CancellationToken ct = default)
    {
        try
        {
            // Проверяем уникальность имени и логина
            var existingUser = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name == user.Name || u.Login == user.Login, ct);

            if (existingUser != null)
            {
                if (existingUser.Name == user.Name)
                    return OperationResult.Error($"Пользователь с именем '{user.Name}' уже существует");
                else
                    return OperationResult.Error($"Пользователь с логином '{user.Login}' уже существует");
            }

            await db.Users.AddAsync(user, ct);
            await db.SaveChangesAsync(ct);

            return OperationResult.Ok();
        }
        catch (OperationCanceledException)
        {
            return OperationResult.Error("Операция была отменена");
        }
        catch (DbUpdateException ex)
        {
            return OperationResult.Error($"Ошибка при сохранении пользователя: {ex.Message}");
        }
        catch (Exception ex)
        {
            return OperationResult.Error($"Неожиданная ошибка: {ex.Message}");
        }
    }

    public async Task<OperationResult<User>> GetByNameAsync(string userName, CancellationToken ct = default)
    {
        try
        {
            var user = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name == userName, ct);

            return user != null
                ? OperationResult<User>.Ok(user)
                : OperationResult<User>.Error($"Пользователь с именем '{userName}' не найден");
        }
        catch (OperationCanceledException)
        {
            return OperationResult<User>.Error("Операция была отменена");
        }
        catch (Exception ex)
        {
            return OperationResult<User>.Error($"Ошибка при поиске пользователя: {ex.Message}");
        }
    }
}