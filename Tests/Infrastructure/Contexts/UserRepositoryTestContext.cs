using Domain.Entities;
using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure;

public class UserRepositoryTestContext : AppDbContext
{
    public UserRepositoryTestContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<Game>();
        modelBuilder.Ignore<Room>();
        modelBuilder.Ignore<Question>();
        modelBuilder.Ignore<PlayerAnswer>();
        modelBuilder.Ignore<Player>();
    }
}
