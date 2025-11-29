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

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await SaveAsync(user);
    }

    public async Task<User?> GetByNameAsync(string userName, CancellationToken ct = default)
    {
        return await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(user => user.PlayerName == userName, ct);
    }
}