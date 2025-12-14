using Domain.Entities;
using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

public class PlayerRepositoryTestContext : AppDbContext
{
    public PlayerRepositoryTestContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<Game>();
        modelBuilder.Ignore<Question>();
        modelBuilder.Ignore<PlayerAnswer>();
    }
}