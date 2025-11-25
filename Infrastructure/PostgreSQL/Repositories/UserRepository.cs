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

    public async Task SaveAsync(User entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await db.Users.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task SaveAsync(IEnumerable<User> entities, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await db.Users.AddRangeAsync(entities, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id, ct);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Users.AsNoTracking().ToListAsync(ct);
    }

    public async Task<User> UpdateOrInsertAsync(User entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var exists = await db.Users.AnyAsync(user => user.Id == entity.Id, ct);
        if (exists)
            db.Users.Update(entity);
        else
            await db.Users.AddAsync(entity, ct);

        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<User?> UpdateOrThrowAsync(User entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await db.Users.FindAsync([entity.Id], ct);
        if (existing is null)
            throw new InvalidOperationException($"User with id: {entity.Id} does not exist");

        db.Users.Update(entity);

        await db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task DeleteAsync(User entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        db.Users.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await GetByIdAsync(id, ct);
        if (user is not null)
            db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(IEnumerable<User> entities, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        db.Users.RemoveRange(entities);
        await db.SaveChangesAsync(ct);
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